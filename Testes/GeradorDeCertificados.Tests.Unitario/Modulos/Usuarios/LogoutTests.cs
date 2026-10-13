using GeradorCertificados.Aplicacao.Modulos.Usuarios;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Usuarios;

[TestClass]
public sealed class LogoutTests
{
    [TestMethod]
    public async Task Deve_EncerrarSessaoDoUsuarioAutenticado()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IProvedorDeUsuario> provedorDeUsuario = new();
        provedorDeUsuario.Setup(provedor => provedor.Id).Returns(usuarioId);

        LogoutUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            provedorDeUsuario.Object);

        // Act
        var resultado = await handler.Handle(new LogoutUsuarioCommand(), default);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        gerenciadorDeIdentidade.Verify(
            gerenciador => gerenciador.LogoutAsync(usuarioId),
            Times.Once);
    }

    [TestMethod]
    public async Task Deve_RetornarFalhaESemEncerrarSessao_QuandoUsuarioNaoEstiverAutenticado()
    {
        // Arrange
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IProvedorDeUsuario> provedorDeUsuario = new();
        provedorDeUsuario.Setup(provedor => provedor.Id).Returns((Guid?)null);

        LogoutUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            provedorDeUsuario.Object);

        // Act
        var resultado = await handler.Handle(new LogoutUsuarioCommand(), default);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        gerenciadorDeIdentidade.Verify(
            gerenciador => gerenciador.LogoutAsync(It.IsAny<Guid>()),
            Times.Never);
    }
}
