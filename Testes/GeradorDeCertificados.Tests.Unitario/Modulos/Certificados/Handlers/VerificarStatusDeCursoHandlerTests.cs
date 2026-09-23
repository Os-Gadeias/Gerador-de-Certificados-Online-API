using GeradorCertificados.Aplicacao.Modulos.Certificados;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Certificados.Handlers;

[TestClass]
public class VerificarStatusDeCursoHandlerTests
{
    [TestMethod]
    public async Task SelecionarStatus_DeCursoDisponivel_RetornaStatus()
    {
        Curso curso = new
            ("Academia do programador",
            "Curso de programation",
            900,
            DateTime.Now.AddDays(30)
                );

        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(curso);

        VerificarStatusCursoQueryHandler verificarStatusCursoQuery = new(
            repositorioCurso.Object
        );

        var resultado = await verificarStatusCursoQuery.Handle(new(curso.Id));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(StatusCurso.Disponivel, resultado.Value.StatusCurso);

    }
    [TestMethod]
    public async Task SelecionarStatus_DeCursoGerandoCertificados_RetornaStatus()
    {
        Curso curso = new
            ("Academia do programador",
            "Curso de programation",
            900,
            DateTime.Now.AddDays(30)
                );

        curso.AlterarParaGerandoCertificados();

        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(curso);

        VerificarStatusCursoQueryHandler verificarStatusCursoQuery = new(
            repositorioCurso.Object
        );

        var resultado = await verificarStatusCursoQuery.Handle(new(curso.Id));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(StatusCurso.GerandoCertificados, resultado.Value.StatusCurso);

    }
    [TestMethod]
    public async Task SelecionarStatus_ComCursoInvalido_RetornaErro()
    {

        Mock<IRepositorioCurso> repositorioCurso = new();

        repositorioCurso.Setup(r =>
            r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Curso?)null);

        VerificarStatusCursoQueryHandler verificarStatusCursoQuery = new(
            repositorioCurso.Object
        );

        var resultado = await verificarStatusCursoQuery.Handle(new(Guid.CreateVersion7()));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não foi encontrado", resultado.Errors.First().Message);

    }
}
