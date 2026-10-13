using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<CadastrarUsuarioResponse>> Cadastrar(
        CadastrarUsuarioRequest req,
        CancellationToken cancellationToken)
    {
        var resultado =
            await mediator.Send(new CadastrarUsuarioCommand(req.Email, req.Senha), cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { usuarioId = resultado.Value.Id },
            new CadastrarUsuarioResponse(
                resultado.Value.Id,
                resultado.Value.NomeUsuario,
                resultado.Value.AccessToken
            )
        );
    }

    [HttpPost("login")]
    [ProducesResponseType<AutenticarUsuarioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<AutenticarUsuarioDto>> Login(LogarUsuarioRequest req, CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(new AutenticarUsuarioCommand(
            req.Email,
            req.Senha
        ), cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado.ToResult());

        return Ok(new AutenticarUsuarioDto(
            resultado.Value.ClienteId,
            req.Email.Split('@')[0],
            resultado.Value.AccessToken,
            resultado.Value.DataExpiracaoEmUtc
        ));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new LogoutUsuarioCommand(),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return NoContent();
    }
}
