using MassTransit;
using Microsoft.Extensions.Logging;

namespace GeradorCertificados.Aplicacao.Consumers;

public class SolicitarCertificadoComsumer
    (
        ILogger<SolicitarCertificadoComsumer> logger
    ) : IConsumer
{

}