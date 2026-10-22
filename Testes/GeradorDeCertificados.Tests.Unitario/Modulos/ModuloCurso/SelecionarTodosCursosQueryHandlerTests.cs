using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.ModuloCurso;

[TestClass]
public sealed class SelecionarTodosCursosQueryHandlerTests
{
    [TestMethod]
    public async Task Deve_RetornarListaDeCursosDoRepositorio()
    {
        // Arrange
        Curso primeiroCurso = new(
            "C# para iniciantes",
            "Fundamentos da linguagem",
            40,
            new DateTime(2026, 11, 1));
        Curso segundoCurso = new(
            "ASP.NET Core",
            "Desenvolvimento de APIs",
            60,
            new DateTime(2026, 12, 1));
        primeiroCurso.AddCertificados([
            new Certificado("Aluno 1", primeiroCurso),
            new Certificado("Aluno 2", primeiroCurso)
        ]);
        segundoCurso.AddCertificados([]);
        Curso terceiroCurso = new(
            "SQL para iniciantes",
            "Fundamentos de banco de dados",
            20,
            new DateTime(2026, 12, 15));
        List<Curso> cursos = [primeiroCurso, segundoCurso, terceiroCurso];

        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso
            .Setup(repositorio => repositorio.SelecionarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursos);

        SelecionarTodosCursosQueryHandler handler = new(repositorioCurso.Object);

        // Act
        var resultado = await handler.Handle(
            new SelecionarTodosCursosQuery(),
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(3, resultado.Value);
        Assert.AreEqual(primeiroCurso.Id, resultado.Value[0].Id);
        Assert.AreEqual(primeiroCurso.Nome, resultado.Value[0].Nome);
        Assert.AreEqual(primeiroCurso.Descricao, resultado.Value[0].Descricao);
        Assert.AreEqual(primeiroCurso.CargaHoraria, resultado.Value[0].CargaHoraria);
        Assert.AreEqual(primeiroCurso.DataConclusao, resultado.Value[0].DataConclusao);
        Assert.AreEqual(2, resultado.Value[0].QntdCertificados);
        Assert.AreEqual(segundoCurso.Id, resultado.Value[1].Id);
        Assert.AreEqual(segundoCurso.Nome, resultado.Value[1].Nome);
        Assert.AreEqual(0, resultado.Value[1].QntdCertificados);
        Assert.AreEqual(terceiroCurso.Id, resultado.Value[2].Id);
        Assert.IsNull(resultado.Value[2].QntdCertificados);
        repositorioCurso.Verify(
            repositorio => repositorio.SelecionarTodosAsync(CancellationToken.None),
            Times.Once);
    }

    [TestMethod]
    public async Task Deve_RetornarListaVazia_QuandoNaoHouverCursos()
    {
        // Arrange
        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso
            .Setup(repositorio => repositorio.SelecionarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        SelecionarTodosCursosQueryHandler handler = new(repositorioCurso.Object);

        // Act
        var resultado = await handler.Handle(
            new SelecionarTodosCursosQuery(),
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsEmpty(resultado.Value);
    }
}
