using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Usuarios;

public sealed record CadastrarUsuarioCommand(string Email, string Senha) : IRequest<Result<Guid>>;
public sealed class CadastrarUsuarioCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<CadastrarUsuarioCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CadastrarUsuarioCommand command, CancellationToken cancellationToken)
    {
        Guid id = Guid.CreateVersion7();

        try
        {
            var usuarioId = await gerenciadorDeIdentidade.CadastrarAsync(
                id,
                command.Email,
                command.Senha
                );

            return Result.Ok(usuarioId);
        }
        catch (ConflitoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosUsuario.ConflitoDeIdentidade(ex.Message));
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosUsuario.ValidacaoDeIdentidade(ex.Campo, ex.Message));
        }
        catch (ConflitoDePersistenciaException)
        {
            await gerenciadorDeIdentidade.ExcluirAsync(id);

            return Result.Fail(ErrosUsuario.CadastroDuplicado());
        }
    }
}
