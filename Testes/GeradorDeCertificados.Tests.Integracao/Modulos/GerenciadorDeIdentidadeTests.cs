using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorDeCertificados.Tests.Integracao.Compartilhado;

namespace GeradorDeCertificados.Tests.Integracao.Modulos;

[TestClass]
public class GerenciadorDeIdentidadeTests : RepositorioTestOrmBase
{
    [TestMethod]
    public async Task NaoDeve_RetornarErro_CadastroDeUsuario_Persiste_ERetornaNoSelecionar()
    {
        var usuarioId = Guid.CreateVersion7();
        string email = "Teste@gmail.com";
        string senha = "Teste@123";

        await gerenciadorDeIdentidade.CadastrarAsync(usuarioId, email, senha);

        dbContext.ChangeTracker.Clear();

        var idUsuarioCadastrado = await gerenciadorDeIdentidade.SelecionarIdAsync(usuarioId);

        Assert.IsNotNull(idUsuarioCadastrado);
        Assert.AreEqual(usuarioId, idUsuarioCadastrado!.Id);
        Assert.AreEqual(email, idUsuarioCadastrado!.Email);
    }
    [TestMethod]
    public async Task Deve_LancarErro_CadastroDeUsuario_ComSenha_MenorQue_OitoCaracteres()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        string email = "teste@gmail.com";
        string senha = "Teste@1";

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
    
}
