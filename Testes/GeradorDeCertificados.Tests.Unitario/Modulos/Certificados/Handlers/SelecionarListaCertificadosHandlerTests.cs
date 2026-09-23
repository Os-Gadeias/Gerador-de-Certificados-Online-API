using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Certificados.Handlers;

[TestClass]
public class SelecionarListaCertificadosHandlerTests
{
    [TestMethod]
    public async Task SelecionarCertificados_PorCurso_RetornaNaListagem()
    {
        Curso curso = new
        ("Academia do programador",
        "Curso de programation",
        900,
        DateTime.Now.AddDays(30)
        );

        List<Certificado> certificados = [
            new Certificado("aluno1", curso),
            new Certificado("aluno2", curso)
        ];

        curso.AddCertificados(certificados);

        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso.Setup(r => r.SelecionarPorIdAsync(curso.Id))
            .ReturnsAsync(curso);

        SolicitaListaCertificadosHandler solicitaListaCertificados =
            new(repositorioCurso.Object);

        var resultado = await solicitaListaCertificados.Handle(new(curso.Id));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(2, resultado.Value.Certificados);
    }

    [TestMethod]
    public async Task SelecionarCertificados_SemCursoExistente_RetornaErro()
    {
        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso.Setup(r => r.SelecionarPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Curso?)null);

        SolicitaListaCertificadosHandler solicitaListaCertificados =
            new(repositorioCurso.Object);

        var resultado = await solicitaListaCertificados.Handle(new(Guid.CreateVersion7()));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não foi encontrado", resultado.Errors.First().Message);
    }

    [TestMethod]
    public async Task SelecionarCertificados_ComCurso_SemCertificados_RetornaUmaListaVazia()
    {
        Curso curso = new
        ("Academia do programador",
        "Curso de programation",
        900,
        DateTime.Now.AddDays(30)
        );

        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso.Setup(r => r.SelecionarPorIdAsync(curso.Id))
            .ReturnsAsync(curso);

        SolicitaListaCertificadosHandler solicitaListaCertificados =
            new(repositorioCurso.Object);

        var resultado = await solicitaListaCertificados.Handle(new(curso.Id));

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não possui", resultado.Errors.First().Message);
    }
}
