using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificados.WebApi.Modulos.Cursos;

[ApiController]
[Route("api/curso")]
public class CursoController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CadastrarCursoResponce>> CadastrarCurso(CadastrarCursoRequest request)
    {
        var result = await mediator.Send(new CadastrarCursoCommand(
            request.Nome,
            request.Descricao,
            request.CargaHoraria,
            request.DataConclusao));

        if (result.IsFailed)
        {
            return this.ProblemDetails(result);
        }

        return new CadastrarCursoResponce(result.Value);
    }
}