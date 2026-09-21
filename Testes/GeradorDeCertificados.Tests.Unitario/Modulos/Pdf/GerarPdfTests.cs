using GeradorCertificados.Aplicacao.PdfGeneretor;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
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
        string nomeAluno = "Thiago Kovalski";
        var curso = new Curso(
            "Academia Progamation",
            "Curso cheio de malucos que mexem no computer",
            900,
            DateTime.Now.AddDays(30)
        );
        var certificado = GerarPdf.Gerar(new Certificado(nomeAluno, curso));

        string textoDoPdf = TextoLerDocumento(certificado);

        Assert.Contains(curso.Nome, textoDoPdf);
        Assert.Contains(curso.Descricao!, textoDoPdf);
        Assert.Contains(curso.CargaHoraria.ToString(), textoDoPdf);
        Assert.Contains(curso.DataConclusao.ToString(), textoDoPdf);
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
