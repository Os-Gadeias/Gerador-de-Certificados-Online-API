using GeradorCertificados.Aplicacao.PdfGeneretor;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MassTransit;

namespace GeradorCertificados.Aplicacao.Consumers;

public class SolicitarCertificadoComsumer
    (
        IRepositorioCertificado repositorioCertificado,
        IRepositorioCurso repositorioCurso
    ) : IConsumer<SolicitarCertificadosMessages>
{
    public async Task Consume(
        ConsumeContext<SolicitarCertificadosMessages> context
        )
    {
        var mensagem = context.Message;

        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(mensagem.IdCurso)
            ?? throw new NullReferenceException();

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

                certificadoSelecionado.AddCaminhoDePdf(pdfCertificado);

                await repositorioCertificado.EditarAsync(
                    certificadoSelecionado.Id,
                    certificadoSelecionado
                );
            }
            catch
            {
                certificadoSelecionado.AlterarParaFalha();
                await repositorioCertificado.EditarAsync(
                    certificadoSelecionado.Id,
                    certificadoSelecionado
                );
                return;
            }

        }
    }
}

