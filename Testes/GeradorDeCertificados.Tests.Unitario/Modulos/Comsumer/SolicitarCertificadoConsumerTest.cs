using GeradorCertificados.Aplicacao.Consumers;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using GeradorCertificados.Dominio.Modulos.Pdf;
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
        List<StatusGeracaoCertificado> statusPersistidos = [];
        repositorioCertificado.Setup(r => r.EditarAsync(
                certificado.Id, certificado, It.IsAny<CancellationToken>()))
            .Callback<Guid, Certificado, CancellationToken>((_, c, _) => statusPersistidos.Add(c.Status))
            .ReturnsAsync(true);
        repositorioCertificado.Setup(r =>
            r.SelecionarPorIdAsync(certificado.Id)).ReturnsAsync(certificado);

        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(curso.Id)).ReturnsAsync(curso);

        repositorioCertificado.Setup(r =>
            r.SelecionarPorCursoAsync(curso.Id)).ReturnsAsync([certificado]);

        Mock<IProvedorDeUsuario> provedorDeUsuario = new();
        Mock<IGerarPdf> gerarPdf = new();
        gerarPdf.Setup(g => g.Gerar(certificado)).Returns([1, 2, 3]);

        SolicitarCertificadoComsumer consumer = new(
            repositorioCertificado.Object,
            repositorioCurso.Object,
            provedorDeUsuario.Object,
            gerarPdf.Object
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
        Assert.AreEqual(StatusGeracaoCertificado.Concluído, certificado.Status);
        CollectionAssert.AreEqual(new[] {
            StatusGeracaoCertificado.GerandoCertificado,
            StatusGeracaoCertificado.GerandoZip,
            StatusGeracaoCertificado.Concluído
        }, statusPersistidos);
    }
    [TestMethod]
    public async Task DeveContinuarLote_QuandoUmPdfFalhar_EReprocessarSomenteFalha()
    {
        var cenario = CriarCenario();
        var falhou = cenario.Certificados[0];
        var sucesso = cenario.Certificados[1];
        cenario.GerarPdf.Setup(g => g.Gerar(falhou)).Throws(new InvalidOperationException("Falha no PDF"));

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => cenario.Consumer.Consume(cenario.Context.Object));

        Assert.AreEqual(StatusGeracaoCertificado.Falha, falhou.Status);
        Assert.AreEqual(StatusGeracaoCertificado.Concluído, sucesso.Status);
        Assert.AreEqual(StatusCurso.Falha, cenario.Curso.Status);
        using (var zip = System.IO.Compression.ZipFile.OpenRead(cenario.Curso.CaminhoZip!))
        {
            Assert.HasCount(1, zip.Entries);
            Assert.AreEqual(Path.GetFileName(sucesso.CaminhoPdf), zip.Entries[0].Name);
        }

        cenario.GerarPdf.Setup(g => g.Gerar(falhou)).Returns([1, 2, 3]);
        await cenario.Consumer.Consume(cenario.Context.Object);

        Assert.IsTrue(cenario.Certificados.All(c => c.Status == StatusGeracaoCertificado.Concluído));
        Assert.AreEqual(StatusCurso.Disponivel, cenario.Curso.Status);
        cenario.GerarPdf.Verify(g => g.Gerar(sucesso), Times.Once);
        using var zipFinal = System.IO.Compression.ZipFile.OpenRead(cenario.Curso.CaminhoZip!);
        Assert.HasCount(2, zipFinal.Entries);
    }

    [TestMethod]
    public async Task DeveMarcarFalhaNosNovosCertificados_QuandoZipFalhar_SemAlterarConcluidos()
    {
        var cenario = CriarCenario();
        var antigo = new Certificado("Aluno antigo", cenario.Curso);
        antigo.AddCaminhoDePdf(Path.Combine("storage", $"inexistente-{Guid.NewGuid()}.pdf"));
        antigo.AlterarParaConcluido();
        cenario.Certificados.Add(antigo);

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => cenario.Consumer.Consume(cenario.Context.Object));

        Assert.AreEqual(StatusGeracaoCertificado.Falha, cenario.Certificados[0].Status);
        Assert.AreEqual(StatusGeracaoCertificado.Falha, cenario.Certificados[1].Status);
        Assert.AreEqual(StatusGeracaoCertificado.Concluído, antigo.Status);
        Assert.AreEqual(StatusCurso.Falha, cenario.Curso.Status);
        Assert.IsNull(cenario.Curso.CaminhoZip);
    }

    [TestMethod]
    public async Task NaoDeveRegerarCertificadosConcluidos_EmReentrega()
    {
        var cenario = CriarCenario();
        await cenario.Consumer.Consume(cenario.Context.Object);
        await cenario.Consumer.Consume(cenario.Context.Object);

        foreach (var certificado in cenario.Certificados)
        {
            Assert.AreEqual(StatusGeracaoCertificado.Concluído, certificado.Status);
            cenario.GerarPdf.Verify(g => g.Gerar(certificado), Times.Once);
        }
        Assert.AreEqual(StatusCurso.Disponivel, cenario.Curso.Status);
    }

    [TestMethod]
    public async Task DeveMarcarTodosComoFalha_QuandoTodosPdfsFalharem()
    {
        var cenario = CriarCenario();
        cenario.GerarPdf.Setup(g => g.Gerar(It.IsAny<Certificado>()))
            .Throws(new InvalidOperationException("Falha no PDF"));

        var erro = await Assert.ThrowsExactlyAsync<AggregateException>(
            () => cenario.Consumer.Consume(cenario.Context.Object));

        Assert.HasCount(2, erro.InnerExceptions);
        Assert.IsTrue(cenario.Certificados.All(c => c.Status == StatusGeracaoCertificado.Falha));
        Assert.AreEqual(StatusCurso.Falha, cenario.Curso.Status);
        Assert.IsNull(cenario.Curso.CaminhoZip);
    }

    [TestMethod]
    public async Task DeveRestaurarZipAusente_SemRegerarPdfsConcluidos()
    {
        var cenario = CriarCenario();
        await cenario.Consumer.Consume(cenario.Context.Object);
        File.Delete(cenario.Curso.CaminhoZip!);

        await cenario.Consumer.Consume(cenario.Context.Object);

        Assert.IsTrue(File.Exists(cenario.Curso.CaminhoZip));
        foreach (var certificado in cenario.Certificados)
            cenario.GerarPdf.Verify(g => g.Gerar(certificado), Times.Once);
    }

    private static (Curso Curso, List<Certificado> Certificados, Mock<IGerarPdf> GerarPdf,
        SolicitarCertificadoComsumer Consumer, Mock<ConsumeContext<SolicitarCertificadosMessages>> Context) CriarCenario()
    {
        var curso = new Curso($"Teste-{Guid.NewGuid()}", "Teste", 10, DateTime.Now);
        List<Certificado> certificados = [new("Aluno um", curso), new("Aluno dois", curso)];
        foreach (var certificado in certificados)
            certificado.AlterarParaPendente();
        var repositorioCurso = new Mock<IRepositorioCurso>();
        repositorioCurso.Setup(r => r.SelecionarPorIdAsync(curso.Id)).ReturnsAsync(curso);
        var repositorioCertificado = new Mock<IRepositorioCertificado>();
        repositorioCertificado.Setup(r => r.SelecionarPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => certificados.SingleOrDefault(c => c.Id == id));
        repositorioCertificado.Setup(r => r.SelecionarPorCursoAsync(curso.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificados);
        var gerarPdf = new Mock<IGerarPdf>();
        gerarPdf.Setup(g => g.Gerar(It.IsAny<Certificado>())).Returns([1, 2, 3]);
        var consumer = new SolicitarCertificadoComsumer(repositorioCertificado.Object,
            repositorioCurso.Object, Mock.Of<IProvedorDeUsuario>(), gerarPdf.Object);
        var context = new Mock<ConsumeContext<SolicitarCertificadosMessages>>();
        context.Setup(c => c.Message).Returns(new SolicitarCertificadosMessages(
            curso.Id, certificados.Select(c => c.Id).ToList(), Guid.NewGuid()));
        return (curso, certificados, gerarPdf, consumer, context);
    }

    [TestMethod]
    public async Task DeveLancarExcecao_QuandoCurso_NaoExiste()
    {
        var idCurso = Guid.CreateVersion7();
        var idUsuario = Guid.CreateVersion7();

        Mock<IRepositorioCurso> repositorioCurso = new();

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        Mock<IProvedorDeUsuario> provedorDeUsuario = new();
        Mock<IGerarPdf> gerarPdf = new();

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
            provedorDeUsuario.Object,
            gerarPdf.Object
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
        Mock<IGerarPdf> gerarPdf = new();

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
            provedorDeUsuario.Object,
            gerarPdf.Object
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
