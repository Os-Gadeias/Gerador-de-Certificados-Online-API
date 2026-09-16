using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios;
using GeradorCertificados.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificados.WebApi.Modulos.Usuarios;

[ApiController]
[Route("api/auth")]
public class UsuariosController(
    IMediator mediator
    ) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{usuarioId:guid}")]
    [ProducesResponseType<ObterUsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ObterUsuarioResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ObterUsuarioResponse>> ObterPorId(Guid usuarioId)
    {
        var resultado = await mediator.Send(
                new ObterUsuarioPorIdQuery(usuarioId)
            );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(new ObterUsuarioResponse(
            resultado.Value.Id,
            resultado.Value.Email)
            );
    }

    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType<CadastrarUsuarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<CadastrarUsuarioResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<CadastrarUsuarioResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CadastrarUsuarioResponse>> Cadastrar(
        CadastrarUsuarioRequest req,
        CancellationToken cancellationToken)
    {
        Result<Guid> resultado =
            await mediator.Send(new CadastrarUsuarioCommand(req.Email, req.Senha), cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { usuarioId = resultado.Value },
            new CadastrarUsuarioResponse(
                resultado.Value
            )
        );
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(CadastrarUsuarioRequest req)
    {
        return Ok();
    }
}
