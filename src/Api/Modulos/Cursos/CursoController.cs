using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Aplicacao.Modulos.Dtos.Cursos;
using GeradorCertificados.WebApi.Compartilhado.Http;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificados.WebApi.Modulos.Cursos;

[ApiController]
[Route("api/curso")]
public class CursoController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CadastrarCursoResponce>(StatusCodes.Status201Created)]
    [ProducesResponseType<CadastrarCursoResponce>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<CadastrarCursoResponce>(StatusCodes.Status400BadRequest)]

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

        return CreatedAtAction(
            nameof(SelecionarPorId),
            new { id = result.Value },
            new CadastrarCursoResponce(result.Value)
        );
    }

    [ProducesResponseType<CadastrarCursoResponce>(StatusCodes.Status200OK)]
    [ProducesResponseType<CadastrarCursoResponce>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<CadastrarCursoResponce>(StatusCodes.Status404NotFound)]

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CursoDto>> SelecionarPorId(Guid id)
    {
        var result = await mediator.Send(new SelecionarCursoPorIdQuery(id));

        if (result.IsFailed)
        {
            return this.ProblemDetails(result);
        }

        return Ok(result.Value);
    }

}