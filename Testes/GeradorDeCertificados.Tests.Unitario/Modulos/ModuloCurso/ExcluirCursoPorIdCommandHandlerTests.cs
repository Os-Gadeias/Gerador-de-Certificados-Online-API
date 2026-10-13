using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.ModuloCurso;

[TestClass]
public sealed class ExcluirCursoPorIdCommandHandlerTests
{
    [TestMethod]
    public async Task Deve_RetornarVerdadeiro_QuandoRepositorioExcluirCurso()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso
            .Setup(repositorio => repositorio.ExcluirAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        ExcluirCursoPorIdCommandHandler handler = new(repositorioCurso.Object);

        // Act
        var resultado = await handler.Handle(
            new ExcluirCursoPorIdCommand(cursoId),
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(resultado.Value);
        repositorioCurso.Verify(
            repositorio => repositorio.ExcluirAsync(cursoId, CancellationToken.None),
            Times.Once);
    }

    [TestMethod]
    public async Task Deve_RetornarFalso_QuandoCursoNaoPertencerAoUsuarioOuNaoExistir()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Mock<IRepositorioCurso> repositorioCurso = new();
        repositorioCurso
            .Setup(repositorio => repositorio.ExcluirAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        ExcluirCursoPorIdCommandHandler handler = new(repositorioCurso.Object);

        // Act
        var resultado = await handler.Handle(
            new ExcluirCursoPorIdCommand(cursoId),
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsFalse(resultado.Value);
    }
}
