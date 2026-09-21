using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CursoTest
{
    [TestMethod]
    public void Deve_Permitir_Nome_Valido()
    {
        var curso = new Curso("Curso de Teste", "Descrição válida", 40, DateTime.Today);

        var erros = curso.Validar();

        Assert.HasCount(0, erros);
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void Deve_Retornar_Erro_Quando_Nome_For_Invalido()
    {
        var curso = new Curso("A", "Descrição válida", 40, DateTime.Today);

        var erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Curso.Nome), erros.First().Campo);
        Assert.AreEqual("O \"Nome\" deve conter entre 2 e 200 caracteres.", erros.First().Mensagem);
    }

    [TestMethod]
    public void Deve_Retornar_Erro_Quando_Descricao_For_Maior_Que_500_Caracteres()
    {
        var curso = new Curso("Curso de Teste", new string('x', 501), 40, DateTime.Today);

        var erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Curso.Descricao), erros.First().Campo);
        Assert.AreEqual("O campo \"Descricao\" deve conter no maximo 500 caracteres", erros.First().Mensagem);
    }

    [TestMethod]
    public void Deve_Retornar_Erro_Quando_Carga_Horaria_For_Invalida()
    {
        var curso = new Curso("Curso de Teste", "Descrição válida", 0, DateTime.Today);

        var erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Curso.CargaHoraria), erros.First().Campo);
        Assert.AreEqual("O campo \"Carga Horaria\" ", erros.First().Mensagem);
    }

    [TestMethod]
    public void Deve_Retornar_Erro_Quando_Data_Conclusao_For_Invalida()
    {
        var curso = new Curso("Curso de Teste", "Descrição válida", 40, DateTime.MinValue);

        var erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Curso.DataConclusao), erros.First().Campo);
        Assert.AreEqual("O campo \"Data de Conclusão\" deve ser  preenchida", erros.First().Mensagem);
    }
    [TestMethod]
    public void AlterarParaStatusParaGerandoCertificados_RetornaCursoComStatusCorreto()
    {
        var curso = new Curso("Curso de Teste", "Descrição válida", 40, DateTime.MinValue);

        curso.AlterarParaGerandoCertificados();

        Assert.AreEqual(StatusCurso.GerandoCertificados, curso.Status);

    }
}