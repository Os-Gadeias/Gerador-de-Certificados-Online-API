using GeradorCertificados.Aplicacao.Modulos.Usuarios;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Usuarios;

[TestClass]
public class AutenticarUsuarioHandlerTests
{
    [TestMethod]
    public async Task Deve_AutenticarUsuario_QuandoCredenciaisForemValidas()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        Guid usuarioId = Guid.CreateVersion7();
        string email = "teste@gmail.com";
        string senha = "123456";
        string token = "token-teste";
        DateTime dataExpiracao = DateTime.UtcNow.AddHours(1);

        UsuarioDto usuario = new(
            usuarioId,
            email
        );

        gerenciadorDeIdentidade
            .Setup(r => r.ChecarValidadeDeSenhaAsync(email, senha))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(r => r.CriarToken(usuarioId, email))
            .Returns(new AccessToken(
                token,
                dataExpiracao
            ));

        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        AutenticarUsuarioCommand command = new(
            email,
            senha
        );

        var resultado = await handler.Handle(command, default);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);

        Assert.AreEqual(usuarioId, resultado.Value.ClienteId);
        Assert.AreEqual(token, resultado.Value.AccessToken);
        Assert.AreEqual(dataExpiracao, resultado.Value.DataExpiracaoEmUtc);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoCredenciaisForemInvalidas()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        string email = "teste@gmail.com";
        string senha = "senha-invalida";

        gerenciadorDeIdentidade
            .Setup(r => r.ChecarValidadeDeSenhaAsync(email, senha))
            .ReturnsAsync((UsuarioDto?)null);


        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        AutenticarUsuarioCommand command = new(
            email,
            senha
        );

        var resultado = await handler.Handle(command, default);

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsFalse(resultado.IsSuccess);

        emissorDeTokens.Verify(
            x => x.CriarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }
    [TestMethod]
    public async Task Deve_PassarEmailESenhaCorretamente_ParaOGerenciadorDeIdentidade()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        Guid usuarioId = Guid.CreateVersion7();
        string email = "teste@gmail.com";
        string senha = "123456";
        string token = "token-teste";
        DateTime dataExpiracao = DateTime.UtcNow.AddHours(1);

        UsuarioDto usuario = new(
            usuarioId,
            email
        );

        gerenciadorDeIdentidade
            .Setup(r => r.ChecarValidadeDeSenhaAsync(email, senha))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(r => r.CriarToken(usuarioId, email))
            .Returns(new AccessToken(
                token,
                dataExpiracao
            ));

        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        AutenticarUsuarioCommand command = new(email, senha);

        await handler.Handle(command, default);

        gerenciadorDeIdentidade.Verify(
            x => x.ChecarValidadeDeSenhaAsync(email, senha),
            Times.Once
        );
    }
    [TestMethod]
    public async Task Deve_CriarTokenComDadosDoUsuario_QuandoCredenciaisForemValidas()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        Guid usuarioId = Guid.CreateVersion7();
        string email = "teste@gmail.com";

        UsuarioDto usuario = new(
            usuarioId,
            email
        );

        gerenciadorDeIdentidade
            .Setup(x => x.ChecarValidadeDeSenhaAsync(email, "123456"))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(x => x.CriarToken(usuarioId, email))
            .Returns(new AccessToken(
                "token-teste",
                DateTime.UtcNow.AddHours(1)
            ));

        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        AutenticarUsuarioCommand command = new(
            email,
            "123456"
        );

        await handler.Handle(command, default);

        emissorDeTokens.Verify(
            x => x.CriarToken(usuarioId, email),
            Times.Once
        );
    }
    [TestMethod]
    public async Task NaoDeve_CriarToken_QuandoCredenciaisForemInvalidas()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        string email = "teste@gmail.com";
        string senha = "senha-invalida";

        gerenciadorDeIdentidade
            .Setup(r => r.ChecarValidadeDeSenhaAsync(email, senha))
            .ReturnsAsync((UsuarioDto?)null);

        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        AutenticarUsuarioCommand command = new(
            email,
            senha
        );

        await handler.Handle(command, default);

        emissorDeTokens.Verify(
            x => x.CriarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }
}