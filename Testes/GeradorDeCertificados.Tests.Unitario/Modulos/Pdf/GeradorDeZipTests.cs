using System.IO.Compression;
using GeradorCertificados.Aplicacao.Consumers;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Pdf;

[TestClass]
public class GeradorDeZipTests
{
    [TestMethod]
    public void SalvarPdfComDadosValidos_CriaArquivoPdfNoDiretorioCorreto()
    {
        Guid idCertificado = Guid.NewGuid();
        byte[] pdf = [1, 2, 3, 4];

        string caminhoPdf = GeradorDeZip.SalvarPdf(pdf, idCertificado);


        Assert.IsTrue(File.Exists(caminhoPdf));
        Assert.AreEqual(
            Path.Combine("storage", "certificados", $"certificado-{idCertificado}.pdf"),
            caminhoPdf
        );
        CollectionAssert.AreEqual(pdf, File.ReadAllBytes(caminhoPdf));

    }

    [TestMethod]
    public void GerarZipComArquivosPdfValidos_CriaZipComTodosOsArquivos()
    {
        string cursoNome = Guid.NewGuid().ToString();
        string diretorio = Path.Combine("storage", "certificados");
        List<string> caminhosPdf = [];

        for (int indice = 1; indice <= 2; indice++)
        {
            string caminhoPdf = Path.Combine(
                diretorio,
                $"teste-{cursoNome}-{indice}.pdf"
            );

            Directory.CreateDirectory(diretorio);
            File.WriteAllBytes(caminhoPdf, [(byte)indice]);
            caminhosPdf.Add(caminhoPdf);
        }

        string caminhoZip = GeradorDeZip.GerarZip(caminhosPdf, cursoNome);


        Assert.IsTrue(File.Exists(caminhoZip));

        using ZipArchive zip = ZipFile.OpenRead(caminhoZip);

        Assert.HasCount(2, zip.Entries);
        Assert.IsNotNull(zip.GetEntry(Path.GetFileName(caminhosPdf[0])));
        Assert.IsNotNull(zip.GetEntry(Path.GetFileName(caminhosPdf[1])));

    }
}
