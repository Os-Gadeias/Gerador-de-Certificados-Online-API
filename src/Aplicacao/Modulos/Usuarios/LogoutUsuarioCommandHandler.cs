using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Usuarios;

public sealed class LogoutUsuarioCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<LogoutUsuarioCommand, Result>
{
    public async Task<Result> Handle(
        LogoutUsuarioCommand command,
        CancellationToken cancellationToken)
    {
        if (provedorDeUsuario.Id is not Guid usuarioId)
            return Result.Fail(ErrosUsuario.NaoAutorizado(Guid.Empty));

        await gerenciadorDeIdentidade.LogoutAsync(usuarioId);

        return Result.Ok();
    }
}
