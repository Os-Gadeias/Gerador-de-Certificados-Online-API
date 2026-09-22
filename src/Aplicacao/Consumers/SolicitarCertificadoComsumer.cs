using System.IO.Compression;
using GeradorCertificados.Aplicacao.PdfGeneretor;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MassTransit;

namespace GeradorCertificados.Aplicacao.Consumers;

public class SolicitarCertificadoComsumer
    (
        IRepositorioCertificado repositorioCertificado,
        IRepositorioCurso repositorioCurso,
        IProvedorDeUsuario provedorDeUsuario
    ) : IConsumer<SolicitarCertificadosMessages>
{
    public async Task Consume(
        ConsumeContext<SolicitarCertificadosMessages> context
        )
    {
        var mensagem = context.Message;
        provedorDeUsuario.DefinirUsuario(mensagem.UsuarioId);

        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(mensagem.IdCurso);

        if (curso is null)
            throw new InvalidOperationException(
                $"Curso {mensagem.IdCurso} não encontrado."
            );

        //Deixa o Status do curso como GerandoCertificados
        curso.AlterarParaGerandoCertificados();

        await repositorioCurso.EditarAsync(
            curso.Id,
            curso);

        //Seleciona cada Certificado e começa a fazer as alteracoes e gerar o certificado
        foreach (Guid idCertificado in mensagem.IdsCertificados)
        {
            var certificadoSelecionado = await repositorioCertificado.SelecionarPorIdAsync(idCertificado)
            ?? throw new NullReferenceException();

            //atualiza o status do certificado para GerandoCertificado
            certificadoSelecionado.AlterarParaGerandoCertificado();

            await repositorioCertificado.EditarAsync(
                certificadoSelecionado.Id,
                certificadoSelecionado
            );
            try
            {
                var pdfCertificado = GerarPdf.Gerar(certificadoSelecionado);

                var caminhoPdf = SalvarPdf(
                pdfCertificado,
                certificadoSelecionado.Id
            );

                certificadoSelecionado.AddCaminhoDePdf(caminhoPdf);

                await repositorioCertificado.EditarAsync(
                    certificadoSelecionado.Id,
                    certificadoSelecionado
                );
            }
            catch (Exception)
            {
                certificadoSelecionado.AlterarParaFalha();
                await repositorioCertificado.EditarAsync(
                    certificadoSelecionado.Id,
                    certificadoSelecionado
                );
                return;
            }

            // Busca somente os certificados dessa solicitação
            List<Certificado> certificados = [];

            foreach (Guid idsCertificados in mensagem.IdsCertificados)
            {
                var certificado =
                    await repositorioCertificado.SelecionarPorIdAsync(idsCertificados)
                    ?? throw new NullReferenceException();

                certificados.Add(certificado);
            }

            //vincula os certificados ao Curso e atualiza o curso no banco
            curso.AddCertificados(certificados);
            await repositorioCurso.EditarAsync(
                curso.Id,
                curso
            );

            // Somente certificados que realmente possuem PDF entram no ZIP
            var caminhosPdf = certificados
                .Where(c =>
                    c.Status == StatusGeracaoCertificado.GerandoCertificado &&
                    c.CaminhoPdf is not null)
                .Select(c => c.CaminhoPdf!)
                .ToList();

            try
            {
                var caminhoZip = GerarZip(
                    caminhosPdf,
                    mensagem.IdCurso
                );

                curso.AdicionarCaminhoDoZip(caminhoZip);
                curso.AlterarParaDisponivel();

                await repositorioCurso.EditarAsync(
                    curso.Id,
                    curso
                );
            }
            catch (Exception)
            {
                curso.AlterarParaFalha();

                await repositorioCurso.EditarAsync(
                    curso.Id,
                    curso
                );
            }
        }
    }
    private static string SalvarPdf(byte[] pdf, Guid idCertificado)
    {
        string diretorio = Path.Combine("storage", "certificados");

        Directory.CreateDirectory(diretorio);

        string nomeArquivo = $"certificado-{idCertificado}.pdf";

        string caminhoDoPdf = Path.Combine(diretorio, nomeArquivo);

        File.WriteAllBytes(caminhoDoPdf, pdf);

        return caminhoDoPdf;
    }
    private static string GerarZip(List<string> caminhosPdf, Guid idCurso)
    {
        var diretorio = Path.Combine(
            "storage",
            "zips"
        );

        Directory.CreateDirectory(diretorio);

        var caminhoZip = Path.Combine(
            diretorio,
            $"curso-{idCurso}.zip"
        );

        using var arquivoZip = new FileStream(
            caminhoZip,
            FileMode.Create
        );

        using var zip = new ZipArchive(
            arquivoZip,
            ZipArchiveMode.Create
        );

        foreach (var caminhoPdf in caminhosPdf)
        {
            zip.CreateEntryFromFile(
                caminhoPdf,
                Path.GetFileName(caminhoPdf)
            );
        }

        return caminhoZip;
    }
}

