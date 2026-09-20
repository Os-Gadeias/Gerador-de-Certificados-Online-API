using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public class Certificado : IDocument
{
    public string NomeAluno { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public DateTime DataConclusao { get; set; }

    public Certificado() { }

    public Certificado(
        string nomeAluno,
        string nomeCurso,
        int cargaHoraria,
        DateTime dataConclusao)
    {
        NomeAluno = nomeAluno;
        NomeCurso = nomeCurso;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public DocumentMetadata GetMetadata()
    {
        return DocumentMetadata.Default;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(2, Unit.Centimeter);
            page.PageColor(Colors.White);

            page.Content().Column(col =>
            {
                col.Spacing(20);

                col.Item()
                    .AlignCenter()
                    .Text("CERTIFICADO DE CONCLUSÃO")
                    .FontSize(28)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                col.Item()
                    .AlignCenter()
                    .Text($"Certificamos que {NomeAluno}")
                    .FontSize(18);

                col.Item()
                    .AlignCenter()
                    .Text($"concluiu com êxito o curso de {NomeCurso}")
                    .FontSize(16)
                    .SemiBold();

                col.Item()
                    .AlignCenter()
                    .Text($"com carga horária total de {CargaHoraria} horas em {DataConclusao:dd/MM/yyyy}.")
                    .FontSize(14);
            });
        });
    }
}