using System.Security.AccessControl;
using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Usuarios;

public sealed record AutenticarUsuarioCommand(
    string Email,
    string Senha
) : IRequest<Result<AutenticarUsuarioDto>>;

public class AutenticarUsuarioCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IEmissorDeTokens emissorDeTokens
    ) : IRequestHandler<AutenticarUsuarioCommand, Result<AutenticarUsuarioDto>>
{
    public async Task<Result<AutenticarUsuarioDto>> Handle(AutenticarUsuarioCommand command, CancellationToken cancellationToken)
    {
        var usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(command.Email, command.Senha);

        if (usuario is null)
            return Result.Fail(ErrosUsuario.CredenciaisInvalidas());

        var accessToken = emissorDeTokens.CriarToken(
            usuario.Id,
            usuario.Email
        );

        return new AutenticarUsuarioDto(
            usuario.Id,
            accessToken.Token,
            accessToken.DataExpiracaoEmUtc
        );
    }
}
