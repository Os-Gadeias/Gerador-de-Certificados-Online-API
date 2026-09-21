using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace GeradorCertificados.Aplicacao.PdfGeneretor;

public static class GerarPdf
{
    public static byte[] Gerar(Certificado certificado)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(50);

                page.Content()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("CERTIFICADO")
                            .FontSize(32)
                            .Bold();

                        column.Item()
                            .PaddingTop(30)
                            .Text("Certificado de Conclusão de Curso");

                        column.Item()
                            .PaddingTop(10)
                            .Text(certificado.NomeAluno)
                            .FontSize(24)
                            .Bold();

                        column.Item()
                            .PaddingTop(20)
                            .Text($"concluiu o curso {certificado.Curso.Nome}: {certificado.Curso.Descricao}.");

                        column.Item()
                            .PaddingTop(10)
                            .Text($"Carga horária: {certificado.Curso.CargaHoraria}");

                        column.Item()
                            .PaddingTop(30)
                            .Text($"Data de conclusão: {certificado.Curso.DataConclusao}");
                    });
            });
        });

        return document.GeneratePdf();
    }
}
