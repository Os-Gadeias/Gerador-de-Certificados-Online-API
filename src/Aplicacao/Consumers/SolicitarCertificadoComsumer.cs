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

        try
        {
            //Seleciona cada Certificado e começa a fazer as alteracoes e gerar o certificado
            foreach (Guid idCertificado in mensagem.IdsCertificados)
            {
                var certificadoSelecionado = await repositorioCertificado.SelecionarPorIdAsync(idCertificado)
                ?? throw new NullReferenceException("Certificado não encontrado.");

                //atualiza o status do certificado para GerandoCertificado
                certificadoSelecionado.AlterarParaGerandoCertificado();

                await repositorioCertificado.EditarAsync(
                    certificadoSelecionado.Id,
                    certificadoSelecionado
                );
                try
                {
                    var pdfCertificado = GerarPdf.Gerar(certificadoSelecionado);

                    var caminhoPdf = GeradorDeZip.SalvarPdf(
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
                    throw;
                }
            }

            // Recarrega todos os certificados do curso para manter o ZIP acumulado.
            List<Certificado> certificados = await repositorioCertificado
                .SelecionarPorCursoAsync(mensagem.IdCurso);

            //vincula os certificados ao Curso e atualiza o curso no banco
            curso.AddCertificados(certificados);
            await repositorioCurso.EditarAsync(curso.Id, curso);

            var caminhosPdf = certificados
                .Where(c => c.CaminhoPdf is not null)
                .Select(c => c.CaminhoPdf)
                .ToList();

            if (caminhosPdf.Any(caminho => caminho is null))
                throw new InvalidOperationException(
                    "Não foi possível gerar o ZIP porque um certificado não possui PDF."
                );

            var caminhoZip = GeradorDeZip.GerarZip(
                caminhosPdf.Select(caminho => caminho!).ToList(),
                curso.Nome
            );

            curso.AdicionarCaminhoDoZip(caminhoZip);
            curso.AlterarParaDisponivel();

            await repositorioCurso.EditarAsync(curso.Id, curso);
        }
        finally
        {
            // deixa o status do curso para uma nova solicitação em sucesso ou erro.
            curso.AlterarParaDisponivel();
            await repositorioCurso.EditarAsync(curso.Id, curso);
        }
    }
}

