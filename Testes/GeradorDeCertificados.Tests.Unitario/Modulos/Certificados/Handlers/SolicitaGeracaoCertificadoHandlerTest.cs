using GeradorCertificados.Aplicacao.Consumers;
using GeradorCertificados.Aplicacao.Modulos.Certificados;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MassTransit;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Certificados.Handlers;

[TestClass]
public class SolicitaGeracaoCertificadoHandlerTest
{
    [TestMethod]
    public async Task SolicitarGeracao_ComCursoValido_E_ListaValida_NaoRetornaErros()
    {
        Curso curso = new
                ("Academia do programador",
                "Curso de programation",
                900,
                DateTime.Now.AddDays(30)
                );

        List<string> nomesAlunos =
        ["Thiago",
        "Victor"];

        Mock<IBus> bus = new();
        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(curso.Id)).ReturnsAsync(curso);

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        SolicitaGeracaoCertificadoCommandHandler solicitaGeracaoCertificado = new(
            repositorioCurso.Object,
            repositorioCertificado.Object,
            bus.Object
            );

        var resultado = await solicitaGeracaoCertificado.Handle(new(nomesAlunos, curso.Id));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(2, resultado.Value.IdCertificados);
        Assert.HasCount(2, resultado.Value.NomesAlunosCertificados);
        Assert.AreEqual("Thiago", resultado.Value.NomesAlunosCertificados[0]);
        Assert.AreEqual("Victor", resultado.Value.NomesAlunosCertificados[1]);
        bus.Verify(b => b.Publish(It.IsAny<SolicitarCertificadosMessages>()), Times.Once);

    }
    [TestMethod]
    public async Task SolicitarGeracao_ComCursoInvalido_RetornaErro()
    {
        List<string> nomesAlunos =
        ["Thiago",
        "Victor"];

        Mock<IBus> bus = new();
        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Curso?)null);

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        SolicitaGeracaoCertificadoCommandHandler solicitaGeracaoCertificado = new(
            repositorioCurso.Object,
            repositorioCertificado.Object,
            bus.Object
            );

        var resultado = await solicitaGeracaoCertificado.Handle(new(nomesAlunos, Guid.CreateVersion7()));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("Curso não encontrado!", resultado.Errors[0].Message);
        bus.Verify(b => b.Publish(It.IsAny<SolicitarCertificadosMessages>()), Times.Never);

    }
    [TestMethod]
    public async Task SolicitarGeracao_ComNomesInvalidos_RetornaErro()
    {
        Curso curso = new
            ("Academia do programador",
            "Curso de programation",
            900,
            DateTime.Now.AddDays(30)
                );

        List<string> nomesAlunos =
        ["T",
        "Vi"];

        Mock<IBus> bus = new();
        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(curso);

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        SolicitaGeracaoCertificadoCommandHandler solicitaGeracaoCertificado = new(
            repositorioCurso.Object,
            repositorioCertificado.Object,
            bus.Object
            );

        var resultado = await solicitaGeracaoCertificado.Handle(new(nomesAlunos, curso.Id));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("O nome:", resultado.Errors[0].Message);
        bus.Verify(b => b.Publish(It.IsAny<SolicitarCertificadosMessages>()), Times.Never);

    }

    [TestMethod]
    public async Task SolicitarGeracao_ComCursoGerandoCertificados_RetornaErro()
    {
        Curso curso = new
            ("Academia do programador",
            "Curso de programation",
            900,
            DateTime.Now.AddDays(30)
                );

        curso.AlterarParaGerandoCertificados();

        List<string> nomesAlunos =
        ["Thiago",
        "Victor"];

        Mock<IBus> bus = new();
        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(curso);

        Mock<IRepositorioCertificado> repositorioCertificado = new();

        SolicitaGeracaoCertificadoCommandHandler solicitaGeracaoCertificado = new(
            repositorioCurso.Object,
            repositorioCertificado.Object,
            bus.Object
            );

        var resultado = await solicitaGeracaoCertificado.Handle(new(nomesAlunos, curso.Id));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("está gerando certificados!", resultado.Errors[0].Message);
        bus.Verify(b => b.Publish(It.IsAny<SolicitarCertificadosMessages>()), Times.Never);
    }
}
