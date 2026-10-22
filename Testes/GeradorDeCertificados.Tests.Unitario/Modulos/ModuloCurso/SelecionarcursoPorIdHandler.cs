using System.Reflection.Metadata;
using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.ModuloCurso;

[TestClass]
public class SelecionarCursoPorIdQuery
{
    [TestMethod]
    public async Task NaoDeve_RetornarErro_CursoCadastrado()
    {
        Curso cursoTest = new Curso(
            "Cadi",
            "Brutal",
            670,
            DateTime.Now.AddDays(30)
        );

        cursoTest.AddCertificados([
            new Certificado("Aluno 1", cursoTest),
            new Certificado("Aluno 2", cursoTest)
        ]);

        Mock<IRepositorioCurso> repositorio = new();

        repositorio.Setup(r => r.SelecionarPorIdAsync(cursoTest.Id)).ReturnsAsync(cursoTest);

        SelecionarCursoPorIdQueryHandler handle = new(repositorio.Object);

        var result = await handle.Handle(new(cursoTest.Id));

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Cadi", result.Value.Nome);
        Assert.AreEqual("Brutal", result.Value.Descricao);
        Assert.AreEqual(670, result.Value.CargaHoraria);
        Assert.AreEqual(cursoTest.DataConclusao, result.Value.DataConclusao);
        Assert.AreEqual(2, result.Value.QntdCertificados);
    }

    [TestMethod]
    public async Task Deve_RetornarQuantidadeZero_QuandoListaDeCertificadosEstiverVazia()
    {
        Curso cursoTest = new("Cadi", "Brutal", 670, DateTime.Now.AddDays(30));
        cursoTest.AddCertificados([]);

        Mock<IRepositorioCurso> repositorio = new();
        repositorio.Setup(r => r.SelecionarPorIdAsync(cursoTest.Id)).ReturnsAsync(cursoTest);

        SelecionarCursoPorIdQueryHandler handle = new(repositorio.Object);

        var result = await handle.Handle(new(cursoTest.Id));

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Value.QntdCertificados);
    }

    [TestMethod]
    public async Task Deve_RetornarQuantidadeNula_QuandoListaDeCertificadosForNula()
    {
        Curso cursoTest = new("Cadi", "Brutal", 670, DateTime.Now.AddDays(30));

        Mock<IRepositorioCurso> repositorio = new();
        repositorio.Setup(r => r.SelecionarPorIdAsync(cursoTest.Id)).ReturnsAsync(cursoTest);

        SelecionarCursoPorIdQueryHandler handle = new(repositorio.Object);

        var result = await handle.Handle(new(cursoTest.Id));

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNull(result.Value.QntdCertificados);
    }

    [TestMethod]
    public async Task Deve_RetornarErro_SemCursoCadastrado()
    {
        Mock<IRepositorioCurso> repositorio = new();

        repositorio.Setup(r => r.SelecionarPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Curso?)null);

        SelecionarCursoPorIdQueryHandler handle = new(repositorio.Object);

        var result = await handle.Handle(new(Guid.CreateVersion7()));

        Assert.IsFalse(result.IsSuccess);
        Assert.Contains("não foi encontrado", result.Errors.First().Message);
    }
}
