using MassTransit;
using Microsoft.Extensions.Logging;

namespace GeradorCertificados.Aplicacao.Consumers;

public class SolicitarCertificadoComsumer
    (
        ILogger<SolicitarCertificadoComsumer> logger
    ) : IConsumer<SolicitarCertificadosMessages>
{
    public async Task Consume(ConsumeContext<SolicitarCertificadosMessages> context)
    {
        var mensagem = context.Message;

        
    }
}

