using System.Net.Mail;
using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Usuarios;

public sealed record ObterUsuarioPorIdQuery(Guid Id) : IRequest<Result<UsuarioDto>>;

public class ObterUsuarioQueryHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<ObterUsuarioPorIdQuery, Result<UsuarioDto>>
{
    public async Task<Result<UsuarioDto>> Handle(ObterUsuarioPorIdQuery query, CancellationToken cancellationToken)
    {
        var usuario = await gerenciadorDeIdentidade.SelecionarIdAsync(query.Id);

        if (usuario is null)
            return Result.Fail(ErrosUsuario.NaoEncontrado(query.Id));

        return new UsuarioDto(usuario.Id, usuario.Email);
    }

}
