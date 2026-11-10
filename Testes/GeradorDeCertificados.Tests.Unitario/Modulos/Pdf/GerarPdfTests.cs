using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Infraestrutura.PdfGeneretor;
using QuestPDF.Infrastructure;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Pdf;

[TestClass]
public class GerarPdfTests
{
    [TestInitialize]
    public void ConfigurarQuestPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [TestMethod]
    public void GerarPdfComDadosValidos_RetornaNoPdfFinal()
    {
        GerarPdf gerarPdf = new();

        string nomeAluno = "Thiago Kovalski";
        var curso = new Curso(
            "Academia Progamation",
            "Curso cheio de malucos que mexem no computer",
            900,
            DateTime.Now.AddDays(30)
        );
        var certificado = gerarPdf.Gerar(new Certificado(nomeAluno, curso));

        string textoDoPdf = TextoLerDocumento(certificado);

        Assert.Contains("CERTIFICADO", textoDoPdf);
        Assert.Contains(curso.Nome, textoDoPdf);
        Assert.Contains(curso.Descricao!, textoDoPdf);
        Assert.Contains(curso.CargaHoraria.ToString(), textoDoPdf);
        Assert.Contains(nomeAluno, textoDoPdf);
        Assert.Contains(curso.DataConclusao.ToString("dd 'de' MMMM 'de' yyyy",
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR")), textoDoPdf);
        using var documento = PdfDocument.Open(certificado);
        Assert.AreEqual(1, documento.NumberOfPages);
        Assert.AreEqual(900, documento.GetPage(1).Width, 0.1);
        Assert.AreEqual(600, documento.GetPage(1).Height, 0.1);
    }

    [TestMethod]
    public void GerarPdfComTextosLongos_MantemUmaPaginaETodosOsDados()
    {
        var nomeAluno = "Maria Alexandra de Albuquerque e Vasconcelos da Silva Santos";
        var curso = new Curso(
            "Desenvolvimento de aplicações completas com arquitetura de software e tecnologias modernas",
            string.Join(" ", Enumerable.Repeat("Conteúdo prático e teórico para desenvolvimento de aplicações.", 8)),
            1200, new DateTime(2026, 11, 10));
        var pdf = new GerarPdf().Gerar(new Certificado(nomeAluno, curso));
        using var documento = PdfDocument.Open(pdf);
        Assert.AreEqual(1, documento.NumberOfPages);
        var texto = string.Join(" ", TextoLerDocumento(pdf).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        Assert.Contains(nomeAluno, texto);
        Assert.Contains(curso.Nome, texto);
        Assert.Contains(curso.Descricao!, texto);
    }

    [TestMethod]
    public void GerarPdfSemDescricao_GeraCertificadoNormalmente()
    {
        var curso = new Curso("Curso sem descrição", null, 20, new DateTime(2026, 11, 10));
        var pdf = new GerarPdf().Gerar(new Certificado("Aluno teste", curso));
        using var documento = PdfDocument.Open(pdf);
        Assert.AreEqual(1, documento.NumberOfPages);
        Assert.Contains("Aluno teste", TextoLerDocumento(pdf));
    }

    private static string TextoLerDocumento(byte[] pdf)
    {
        using PdfDocument documento = PdfDocument.Open(pdf);

        string texto = string.Join(
            Environment.NewLine,
            documento.GetPages()
            .Select(pagina => ContentOrderTextExtractor.GetText(pagina, true))
        );

        return texto;
    }
}
