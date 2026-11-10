using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using GeradorCertificados.Dominio.Modulos.Pdf;
using MassTransit;

namespace GeradorCertificados.Aplicacao.Consumers;

public class SolicitarCertificadoComsumer
    (
        IRepositorioCertificado repositorioCertificado,
        IRepositorioCurso repositorioCurso,
        IProvedorDeUsuario provedorDeUsuario,
        IGerarPdf gerarPdf
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

        List<Certificado> certificadosDoLote = [];
        List<Exception> falhas = [];

        try
        {
            //Seleciona cada Certificado e começa a fazer as alteracoes e gerar o certificado
            foreach (Guid idCertificado in mensagem.IdsCertificados.Distinct())
            {
                Certificado? certificadoSelecionado = null;
                try
                {
                    certificadoSelecionado = await repositorioCertificado.SelecionarPorIdAsync(idCertificado)
                        ?? throw new NullReferenceException("Certificado não encontrado.");

                    if (certificadoSelecionado.Curso.Id != curso.Id)
                        throw new InvalidOperationException("O certificado não pertence ao curso solicitado.");

                    certificadosDoLote.Add(certificadoSelecionado);

                    // Uma reentrega não deve refazer certificados já concluídos.
                    if (certificadoSelecionado.Status == StatusGeracaoCertificado.Concluído
                        && File.Exists(certificadoSelecionado.CaminhoPdf))
                        continue;

                    certificadoSelecionado.AlterarParaGerandoCertificado();
                    await repositorioCertificado.EditarAsync(certificadoSelecionado.Id, certificadoSelecionado);

                    var pdfCertificado = gerarPdf.Gerar(certificadoSelecionado);

                    var caminhoPdf = GeradorDeZip.SalvarPdf(
                    pdfCertificado,
                    certificadoSelecionado.Id
                );

                    certificadoSelecionado.AddCaminhoDePdf(caminhoPdf);
                    certificadoSelecionado.AlterarParaGerandoZip();

                    await repositorioCertificado.EditarAsync(
                        certificadoSelecionado.Id,
                        certificadoSelecionado
                    );
                }
                catch (Exception ex)
                {
                    if (certificadoSelecionado is not null && certificadosDoLote.Contains(certificadoSelecionado))
                    {
                        certificadoSelecionado.AlterarParaFalha();
                        await repositorioCertificado.EditarAsync(
                            certificadoSelecionado.Id,
                            certificadoSelecionado
                        );
                    }
                    falhas.Add(ex);
                }
            }

            // Recarrega todos os certificados do curso para manter o ZIP acumulado.
            List<Certificado> certificados = await repositorioCertificado
                .SelecionarPorCursoAsync(mensagem.IdCurso);

            //vincula os certificados ao Curso e atualiza o curso no banco
            curso.AddCertificados(certificados);
            await repositorioCurso.EditarAsync(curso.Id, curso);

            var aguardandoZip = certificadosDoLote
                .Where(c => c.Status == StatusGeracaoCertificado.GerandoZip)
                .ToList();

            var precisaRestaurarZip = certificadosDoLote.Any(c => c.Status == StatusGeracaoCertificado.Concluído)
                && !File.Exists(curso.CaminhoZip);

            if (aguardandoZip.Count > 0 || precisaRestaurarZip)
            {
                // Inclui os certificados antigos concluídos, mas nunca PDFs de tentativas com falha.
                var certificadosParaZip = certificados
                    .Where(c => c.Status == StatusGeracaoCertificado.Concluído
                        || aguardandoZip.Any(atual => atual.Id == c.Id))
                    .ToList();

                if (certificadosParaZip.Any(c => string.IsNullOrWhiteSpace(c.CaminhoPdf)))
                    throw new InvalidOperationException(
                        "Não foi possível gerar o ZIP porque um certificado não possui PDF."
                    );

                var caminhoZip = GeradorDeZip.GerarZip(
                    certificadosParaZip.Select(c => c.CaminhoPdf!).ToList(),
                    curso.Nome
                );

                curso.AdicionarCaminhoDoZip(caminhoZip);
                await repositorioCurso.EditarAsync(curso.Id, curso);

                foreach (var certificado in aguardandoZip)
                {
                    certificado.AlterarParaConcluido();
                    await repositorioCertificado.EditarAsync(certificado.Id, certificado);
                }
            }

            if (falhas.Count == 1)
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(falhas[0]).Throw();
            if (falhas.Count > 1)
                throw new AggregateException("Falha ao gerar certificados do lote.", falhas);

            curso.AlterarParaDisponivel();
            await repositorioCurso.EditarAsync(curso.Id, curso);
        }
        catch
        {
            // Se o ZIP falhar, apenas os certificados deste lote que aguardavam o ZIP falham.
            foreach (var certificado in certificadosDoLote.Where(c =>
                c.Status is StatusGeracaoCertificado.GerandoZip or StatusGeracaoCertificado.GerandoCertificado))
            {
                certificado.AlterarParaFalha();
                await repositorioCertificado.EditarAsync(certificado.Id, certificado);
            }
            curso.AlterarParaFalha();
            await repositorioCurso.EditarAsync(curso.Id, curso);
            throw;
        }
    }
}

