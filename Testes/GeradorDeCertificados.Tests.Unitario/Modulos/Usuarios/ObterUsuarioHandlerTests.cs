using GeradorCertificados.Aplicacao.Modulos.Usuarios;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Usuarios;

[TestClass]
public class ObterUsuarioHandlerTests
{
    [TestMethod]
    public async Task Deve_RetornarUsuario_QuandoUsuarioExistir()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        UsuarioDto dto = new(
            Guid.CreateVersion7(),
            "Teste@gmail.com"
            );

        gerenciadorDeIdentidade.Setup(r =>
            r.SelecionarIdAsync(It.IsAny<Guid>())).ReturnsAsync(dto);

        ObterUsuarioQueryHandler handler = new(gerenciadorDeIdentidade.Object);

        var resultado = await handler.Handle(new(dto.Id), default);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(dto.Id, resultado.Value.Id);
        Assert.AreEqual(dto.Email, resultado.Value.Email);
    }
    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoUsuarioNaoExistir()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        gerenciadorDeIdentidade.Setup(r =>
            r.SelecionarIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UsuarioDto?)null);

        ObterUsuarioQueryHandler handler = new(gerenciadorDeIdentidade.Object);

        var resultado = await handler.Handle(new(Guid.CreateVersion7()), default);

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains(resultado.Errors.First().Message, "O usuário com este ID não foi encontrado.");
    }
    [TestMethod]
    public async Task Deve_PassarIdCorretamente_ParaOGerenciadorDeIdentidade()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        UsuarioDto dto = new(
            Guid.CreateVersion7(),
            "Teste@gmail.com"
            );

        gerenciadorDeIdentidade.Setup(r =>
            r.SelecionarIdAsync(It.IsAny<Guid>())).ReturnsAsync(dto);

        ObterUsuarioQueryHandler handler = new(gerenciadorDeIdentidade.Object);

        var resultado = await handler.Handle(new(dto.Id), default);

        gerenciadorDeIdentidade.Verify(r => r.SelecionarIdAsync(dto.Id), Times.Once);
    }
}
