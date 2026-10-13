using GeradorDeCertificados.Tests.Integracao.Compartilhado;
using Microsoft.AspNetCore.Identity;

namespace GeradorDeCertificados.Tests.Integracao.Modulos;

[TestClass]
public sealed class LogoutTests : RepositorioTestOrmBase
{
    [TestMethod]
    public async Task Deve_AtualizarSecurityStamp_SemExcluirUsuario()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();
        await gerenciadorDeIdentidade.CadastrarAsync(
            usuarioId,
            "logout@example.com",
            "Teste@123");
        IdentityUser<Guid> usuario = (await userManager.FindByIdAsync(usuarioId.ToString()))!;
        string securityStampAnterior = usuario.SecurityStamp!;

        // Act
        await gerenciadorDeIdentidade.LogoutAsync(usuarioId);

        // Assert
        dbContext.ChangeTracker.Clear();
        IdentityUser<Guid>? usuarioAtualizado =
            await userManager.FindByIdAsync(usuarioId.ToString());

        Assert.IsNotNull(usuarioAtualizado);
        Assert.AreNotEqual(securityStampAnterior, usuarioAtualizado.SecurityStamp);
        Assert.AreEqual(usuarioId, usuarioAtualizado.Id);
    }
}
