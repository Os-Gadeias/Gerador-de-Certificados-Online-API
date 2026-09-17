using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorDeCertificados.Tests.Integracao.Compartilhado;
using Microsoft.AspNetCore.Identity;

namespace GeradorDeCertificados.Tests.Integracao.Modulos;

[TestClass]
public class GerenciadorDeIdentidadeTests : RepositorioTestOrmBase
{
    [TestMethod]
    public async Task NaoDeve_RetornarErro_CadastroDeUsuario_Persiste_ERetornaNoSelecionar()
    {
        var usuarioId = Guid.CreateVersion7();
        string email = "TesteCorreto@gmail.com";
        string senha = "Teste555@123";

        await gerenciadorDeIdentidade.CadastrarAsync(usuarioId, email, senha);

        dbContext.ChangeTracker.Clear();

        var idUsuarioCadastrado = await gerenciadorDeIdentidade.SelecionarIdAsync(usuarioId);

        Assert.IsNotNull(idUsuarioCadastrado);
        Assert.AreEqual(usuarioId, idUsuarioCadastrado!.Id);
        Assert.AreEqual(email, idUsuarioCadastrado!.Email);
    }

    [TestMethod]
    public async Task Deve_LancarErro_CadastroDeUsuario_ComEmail_Invalido()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        string email = "teste";
        string senha = "Teste@1234";

        // Act
        try
        {
            await gerenciadorDeIdentidade.CadastrarAsync(
                usuarioId,
                email,
                senha);

            Assert.Fail("Era esperada uma ValidacaoDeIdentidadeException.");
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            // Assert
            Assert.AreEqual("Email", ex.Campo);
        }
    }
    [TestMethod]
    public async Task Deve_LancarErro_CadastroDeUsuario_ComEmail_Vazio()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        string email = string.Empty;
        string senha = "Teste@1234";

        // Act
        try
        {
            await gerenciadorDeIdentidade.CadastrarAsync(
                usuarioId,
                email,
                senha);

            Assert.Fail("Era esperada uma ValidacaoDeIdentidadeException.");
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            // Assert
            Assert.AreEqual("Email", ex.Campo);
        }
    }

    [TestMethod]
    public async Task Deve_LancarErro_CadastroDeUsuario_ComSenha_Vazio()
    {
        // Arrange

        var usuarioId = Guid.CreateVersion7();
        string email = "Teste@gmail.com";
        string senha = string.Empty;

        // Act
        try
        {
            await gerenciadorDeIdentidade.CadastrarAsync(
                usuarioId,
                email,
                senha);

            Assert.Fail("Era esperada uma ValidacaoDeIdentidadeException.");
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            // Assert
            Assert.AreEqual("Senha", ex.Campo);
        }
    }
    [TestMethod]
    public async Task UsuarioCadastrado_RetornaNo_SelecionarPorId()
    {
        //Assert
        var usuario = new IdentityUser<Guid>
        {
            Id = Guid.CreateVersion7(),
            Email = "TesteThiago@gmail.com",
            UserName = "TesteThiago@gmail.com",
        };

        string senha = "Teste@123";

        await userManager.CreateAsync(usuario, senha);

        dbContext.ChangeTracker.Clear();

        var usuarioSelecionado = await gerenciadorDeIdentidade.SelecionarIdAsync(usuario.Id);

        Assert.IsNotNull(usuarioSelecionado);
        Assert.AreEqual(usuario.Id, usuarioSelecionado.Id);
        Assert.AreEqual(usuario.Email, usuarioSelecionado.Email);
    }

    [TestMethod]
    public async Task Usuario_Nao_Cadastrado_RetornaNo_SelecionarPorId()
    {
        var usuarioId = Guid.CreateVersion7();

        var usuarioSelecionado = await gerenciadorDeIdentidade.SelecionarIdAsync(usuarioId);

        Assert.IsNull(usuarioSelecionado);
    }

    [TestMethod]
    public async Task Deve_RetornarUsuario_QuandoEmailESenhaForemValidos()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();
        string email = "testeSenhaInvalida@gmail.com";
        string senha = "Teste@123";

        await gerenciadorDeIdentidade.CadastrarAsync(
            usuarioId,
            email,
            senha);

        // Act
        UsuarioDto? resultado =
            await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
                email,
                senha);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(usuarioId, resultado.Id);
        Assert.AreEqual(email, resultado.Email);
    }

    [TestMethod]
    public async Task Deve_RetornarNulo_QuandoUsuarioNaoExistir()
    {
        // Arrange
        string email = "naoexiste@gmail.com";
        string senha = "Teste@123";

        // Act
        UsuarioDto? resultado =
            await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
                email,
                senha);

        // Assert
        Assert.IsNull(resultado);
    }
}
