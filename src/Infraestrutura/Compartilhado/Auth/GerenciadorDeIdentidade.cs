using GeradorCertificados.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;

namespace GeradorCertificados.Infraestrutura.Compartilhado.Auth;

public sealed class GerenciadorDeIdentidade(
    UserManager<IdentityUser<Guid>> userManager
) : IGerenciadorDeIdentidade
{
    public async Task CadastrarAsync(
        Guid usuarioId,
        string email,
        string senha
    )
    {
        var usuario = new IdentityUser<Guid>
        {
            Id = usuarioId,
            Email = email.Trim(),
            UserName = email.Split('@')[0].Trim()
        };

        var resultadoUsuario = await userManager.CreateAsync(usuario, senha);

        if (!resultadoUsuario.Succeeded)
            throw CriarErro(resultadoUsuario);

        return;
    }

    public async Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha
    )
    {
        var usuario = await userManager.FindByEmailAsync(email);

        if (usuario is null || await userManager.IsLockedOutAsync(usuario))
            return null;

        if (!await userManager.CheckPasswordAsync(usuario, senha))
        {
            await userManager.AccessFailedAsync(usuario);

            return null;
        }

        if (usuario.AccessFailedCount > 0)
            await userManager.ResetAccessFailedCountAsync(usuario);

        return new UsuarioDto(usuario.Id, usuario.Email!);
    }
    public async Task<UsuarioDto?> SelecionarIdAsync(Guid id)
    {
        var resultado = await userManager.FindByIdAsync(id.ToString());

        if (resultado is null)
            return null;

        return new UsuarioDto(resultado.Id, resultado.Email!);
    }
    private static Exception CriarErro(IdentityResult resultado)
    {
        if (resultado.Errors.Any(
                     erro => erro.Code is "DuplicateEmail" or "DuplicateUserName"))
        {
            return new ConflitoDeIdentidadeException(
                "Já existe um usuário cadastrado com este email."
            );
        }

        var erro = resultado.Errors.First();

        string campo = erro.Code.StartsWith("Password", StringComparison.Ordinal)
            ? "Senha"
            : "Email";

        return new ValidacaoDeIdentidadeException(campo, erro.Description);
    }

    public async Task ExcluirAsync(Guid usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());

        if (usuario is not null)
            await userManager.DeleteAsync(usuario);
    }

    public async Task LogoutAsync(Guid usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());

        if (usuario is null)
            return;

        var resultado = await userManager.UpdateSecurityStampAsync(usuario);

        if (!resultado.Succeeded)
        {
            string erros = string.Join(
                "; ",
                resultado.Errors.Select(erro => erro.Description));

            throw new InvalidOperationException(
                $"Não foi possível encerrar a sessão do usuário {usuarioId}: {erros}");
        }
    }
}
