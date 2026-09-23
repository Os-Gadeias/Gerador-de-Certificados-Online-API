using System.IO.Compression;

namespace GeradorCertificados.Aplicacao.Consumers;

public static class GeradorDeZip
{
    public static string SalvarPdf(byte[] pdf, Guid idCertificado)
    {
        string diretorio = Path.Combine(
            "storage",
            "certificados"
            );

        Directory.CreateDirectory(diretorio);

        string nomeArquivo = $"certificado-{idCertificado}.pdf";

        string caminhoDoPdf = Path.Combine(diretorio, nomeArquivo);

        File.WriteAllBytes(caminhoDoPdf, pdf);

        return caminhoDoPdf;
    }
    public static string GerarZip(List<string> caminhosPdf, string cursoNome)
    {
        var diretorio = Path.Combine(
            "storage",
            "zips"
        );

        Directory.CreateDirectory(diretorio);

        var caminhoZip = Path.Combine(
            diretorio,
            $"curso-{cursoNome}.zip"
        );

        using var arquivoZip = new FileStream(
            caminhoZip,
            FileMode.Create
        );

        using var zip = new ZipArchive(
            arquivoZip,
            ZipArchiveMode.Create
        );

        foreach (var caminhoPdf in caminhosPdf)
        {
            zip.CreateEntryFromFile(
                caminhoPdf,
                Path.GetFileName(caminhoPdf)
            );
        }

        return caminhoZip;
    }
}
