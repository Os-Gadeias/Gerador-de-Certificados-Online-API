using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Usuarios;
using GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Usuarios;

[TestClass]
public sealed class CadastrarUsuarioHandlerTests
{
    [TestMethod]
    public async Task Deve_CadastrarUsuario_QuandoDadosForemValidos()
    {
        // Arrange
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        Guid idCapturado = Guid.Empty;
        string emailCapturado = string.Empty;
        string senhaCapturada = string.Empty;

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Callback<Guid, string, string>((id, email, senha) =>
            {
                idCapturado = id;
                emailCapturado = email;
                senhaCapturada = senha;
            })
            .Returns(Task.CompletedTask);

        CadastrarUsuarioCommandHandler handler =
            new(gerenciadorDeIdentidade.Object);

        CadastrarUsuarioCommand command =
            new("teste@gmail.com", "Teste@123");

        // Act
        var resultado = await handler.Handle(command, default);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        Assert.AreNotEqual(Guid.Empty, idCapturado);
        Assert.AreEqual(command.Email, emailCapturado);
        Assert.AreEqual(command.Senha, senhaCapturada);

        Assert.AreEqual(idCapturado, resultado.Value);

        gerenciadorDeIdentidade.Verify(
            g => g.CadastrarAsync(
                idCapturado,
                command.Email,
                command.Senha),
            Times.Once);
    }
    [TestMethod]
    public async Task Deve_RetornarFalhaDeConflito_QuandoIdentidadeEstiverEmConflito()
    {
        // Arrange
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(
                new ConflitoDeIdentidadeException(
                    "Já existe um usuário cadastrado com este email."
                )
            );

        CadastrarUsuarioCommandHandler handler =
            new(gerenciadorDeIdentidade.Object);

        CadastrarUsuarioCommand command =
            new("teste@gmail.com", "123456");

        // Act
        var resultado = await handler.Handle(command, default);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            "Já existe um usuário cadastrado com este email.",
            resultado.Errors.First().Message
        );
    }
    [TestMethod]
    public async Task Deve_RetornarFalhaDeValidacao_QuandoDadosForemInvalidos()
    {
        // Arrange
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync( //configura o cadastrarAsync para retornar uma excecao
                new ValidacaoDeIdentidadeException(
                    "Senha",
                    "A senha deve possuir no mínimo 6 caracteres."
                )
            );

        CadastrarUsuarioCommandHandler handler =
            new(gerenciadorDeIdentidade.Object);

        CadastrarUsuarioCommand command =
            new("teste@gmail.com", "123");

        // Act
        var resultado = await handler.Handle(command, default);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            "A senha deve possuir no mínimo 6 caracteres.",
            resultado.Errors.First().Message
        );
    }
}
