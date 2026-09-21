using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.ModuloCurso;

[TestClass]
public class CadastrarCursoHandler()
{
    [TestMethod]
    public async Task CadastrarCurso_Dadosvalidos_NãoRetorna_Erro()
    {
        Mock<IRepositorioCurso> Rep = new();

        CadastrarCursoCommandHandler Handler = new(Rep.Object);
        var result = await Handler.Handle(new CadastrarCursoCommand("cademia", "brutal", 600, DateTime.Today.AddDays(30)));

        Assert.IsTrue(result.IsSuccess);
        Rep.Verify(r => r.CadastrarAsync(It.IsAny<Curso>()), Times.Once);
    }
}