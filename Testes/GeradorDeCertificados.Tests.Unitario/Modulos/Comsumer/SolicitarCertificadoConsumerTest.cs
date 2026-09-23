using GeradorCertificados.Aplicacao.Consumers;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MassTransit;
using Moq;
using QuestPDF.Infrastructure;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Comsumer;

[TestClass]
public class SolicitarCertificadoConsumerTest
{
    [TestInitialize]
    public void LicensaDoPdfQuest()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
    [TestMethod]
    public async Task ComsumerComDados_MensageValida_()
    {
        Curso curso = new
        ("Academia do programador",
        "Curso de programation",
        900,
        DateTime.Now.AddDays(30)
        );

        Certificado certificado = new("Thiago", curso);

        var usuarioId = Guid.CreateVersion7();

        Mock<IRepositorioCertificado> repositorioCertificado = new();
        repositorioCertificado.Setup(r =>
            r.SelecionarPorIdAsync(certificado.Id)).ReturnsAsync(certificado);

        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(curso.Id)).ReturnsAsync(curso);

        repositorioCertificado.Setup(r =>
            r.SelecionarPorCursoAsync(curso.Id)).ReturnsAsync([certificado]);

        Mock<IProvedorDeUsuario> provedorDeUsuario = new();

        SolicitarCertificadoComsumer consumer = new(
            repositorioCertificado.Object,
            repositorioCurso.Object,
            provedorDeUsuario.Object
        );

        var mensagem = new SolicitarCertificadosMessages(
            curso.Id, [certificado.Id], usuarioId
        );

        Mock<ConsumeContext<SolicitarCertificadosMessages>> context = new();

        context
            .Setup(x => x.Message)
            .Returns(mensagem);

        await consumer.Consume(context.Object);

        Assert.IsNotNull(curso.CaminhoZip);
        Assert.AreEqual(StatusCurso.Disponivel, curso.Status);
    }
    [TestMethod]
    public async Task DeveLancarExcecao_QuandoCurso_NaoExiste()
    {
        var idCurso = Guid.CreateVersion7();
        var idUsuario = Guid.CreateVersion7();

        Mock<IRepositorioCurso> repositorioCurso = new();

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        Mock<IProvedorDeUsuario> provedorDeUsuario = new();

        var mensagem = new SolicitarCertificadosMessages(
            idCurso,
            [],
            idUsuario
        );

        var context =
            new Mock<ConsumeContext<SolicitarCertificadosMessages>>();

        context
            .Setup(x => x.Message)
            .Returns(mensagem);

        ;

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(idCurso))
            .ReturnsAsync((Curso?)null);

        var consumer = new SolicitarCertificadoComsumer(
            repositorioCertificado.Object,
            repositorioCurso.Object,
            provedorDeUsuario.Object
        );

        try
        {
            await consumer.Consume(context.Object);

            Assert.Fail("Era esperado um execeção.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual(
                $"Curso {idCurso} não encontrado.",
                ex.Message
            );
        }
    }
    [TestMethod]
    public async Task DeveLancarExcecao_QuandoCertificado_NaoExiste()
    {
        Curso curso = new
        ("Academia do programador",
        "Curso de programation",
        900,
        DateTime.Now.AddDays(30)
        );

        var idUsuario = Guid.CreateVersion7();

        Mock<IRepositorioCurso> repositorioCurso = new();

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        Mock<IProvedorDeUsuario> provedorDeUsuario = new();

        var mensagem = new SolicitarCertificadosMessages(
            curso.Id,
            [Guid.CreateVersion7()],
            idUsuario
        );

        var context =
            new Mock<ConsumeContext<SolicitarCertificadosMessages>>();

        context
            .Setup(r => r.Message)
            .Returns(mensagem);

        ;

        repositorioCurso
            .Setup(r => r.SelecionarPorIdAsync(curso.Id))
            .ReturnsAsync(curso);

        repositorioCertificado
            .Setup(r => r.SelecionarPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Certificado?)null);

        var consumer = new SolicitarCertificadoComsumer(
            repositorioCertificado.Object,
            repositorioCurso.Object,
            provedorDeUsuario.Object
        );

        try
        {
            await consumer.Consume(context.Object);

            Assert.Fail("Era esperado um execeção.");
        }
        catch (NullReferenceException ex)
        {
            Assert.AreEqual(
                "Certificado não encontrado.",
                ex.Message
            );
        }
    }
}
