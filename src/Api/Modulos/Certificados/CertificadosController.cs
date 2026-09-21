using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificados.WebApi.Modulos.Certificados;

[ApiController]
[Route("api/cursos")]
public class CertificadosController(IMediator mediator) : ControllerBase
{
    [HttpPost("{cursoId:guid}")]
    public async Task<ActionResult<SolicitarGeracaoDeCertificadosResponse>> SolicitarGeracaoCertificados
        (Guid cursoId, SolicitarGeracaoDeCertificadosCommand command)
    {
        var resultado = await mediator.Send(
            new SolicitarGeracaoCommand(command.NomesAlunos, cursoId)
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(
            resultado.Value
        );
    }
}
