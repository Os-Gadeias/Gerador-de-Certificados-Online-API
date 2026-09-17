using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Usuarios;

public sealed class CadastrarUsuarioCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<CadastrarUsuarioCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CadastrarUsuarioCommand command, CancellationToken cancellationToken)
    {
        Guid idUsuarioCadastrado = Guid.CreateVersion7();

        try
        {
            await gerenciadorDeIdentidade.CadastrarAsync(
               idUsuarioCadastrado,
               command.Email,
               command.Senha
               );

            return Result.Ok(idUsuarioCadastrado);
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
            await gerenciadorDeIdentidade.ExcluirAsync(idUsuarioCadastrado);

            return Result.Fail(ErrosUsuario.CadastroDuplicado());
        }
    }
}
