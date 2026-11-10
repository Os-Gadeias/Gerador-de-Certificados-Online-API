using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.Pdf;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace GeradorCertificados.Infraestrutura.PdfGeneretor;

public class GerarPdf : IGerarPdf
{
    private const float Largura = 900;
    private const float Altura = 600;
    private const string Roxo = "#6C2BFF";
    private const string Texto = "#0B1021";
    private const string TextoSecundario = "#59647E";
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");
    private static readonly string Arte = CriarArte();

    public byte[] Gerar(Certificado certificado)
        => CriarDocumento(certificado).GeneratePdf();

    private static Document CriarDocumento(Certificado certificado)
    {
        ArgumentNullException.ThrowIfNull(certificado);
        ArgumentNullException.ThrowIfNull(certificado.Curso);
        var curso = certificado.Curso;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // A referência possui proporção 3:2, diferente do papel A4.
                page.Size(Largura, Altura);
                page.Margin(0);
                page.DefaultTextStyle(style => style.FontFamily("Lato").FontColor(Texto));

                page.Content().Layers(layers =>
                {
                    layers.PrimaryLayer().Svg(Arte);

                    Posicionar(layers.Layer(), 165, 45, 260, 20)
                        .Text("Kovalski").FontSize(15).Bold().LetterSpacing(0.08f);
                    Posicionar(layers.Layer(), 165, 67, 260, 14)
                        .Text("TECH ACADEMY").FontSize(8).FontColor(TextoSecundario).LetterSpacing(0.3f);

                    Posicionar(layers.Layer(), 725, 47, 160, 30)
                        .Text("BETTER DEVELOPERS\nA BRIGHTER FUTURE")
                        .FontSize(7.5f).FontColor(TextoSecundario).LetterSpacing(0.25f).LineHeight(1.7f);

                    Posicionar(layers.Layer(), 163, 107, 565, 48).ScaleToFit()
                        .Text("CERTIFICADO").FontSize(41).ExtraBold().LetterSpacing(0.12f);
                    Posicionar(layers.Layer(), 164, 157, 540, 28)
                        .Text("DE CONCLUSÃO").FontSize(20).FontColor(Roxo).LetterSpacing(0.25f);

                    Posicionar(layers.Layer(), 165, 204, 500, 20)
                        .Text("Certificamos que").FontSize(13).FontColor(TextoSecundario);

                    Posicionar(layers.Layer(), 164, 229, 545, 44).ScaleToFit()
                        .Text(certificado.NomeAluno).FontSize(35).ExtraBold().FontColor(Roxo);

                    Posicionar(layers.Layer(), 165, 298, 545, 22)
                        .Text("concluiu com sucesso o curso").FontSize(13).FontColor(TextoSecundario);
                    Posicionar(layers.Layer(), 165, 322, 545, 31).ScaleToFit()
                        .Text(curso.Nome).FontSize(22).Bold();

                    if (!string.IsNullOrWhiteSpace(curso.Descricao))
                    {
                        Posicionar(layers.Layer(), 165, 354, 545, 42).ScaleToFit()
                            .Text(curso.Descricao).FontSize(12).FontColor(TextoSecundario).LineHeight(1.4f);
                    }

                    Informacao(Posicionar(layers.Layer(), 216, 421, 160, 34), "CURSO", curso.Nome);
                    Informacao(Posicionar(layers.Layer(), 445, 421, 78, 34), "CARGA HORÁRIA", $"{curso.CargaHoraria}h");
                    Informacao(Posicionar(layers.Layer(), 600, 421, 190, 34), "DATA DE CONCLUSÃO",
                        curso.DataConclusao.ToString("dd 'de' MMMM 'de' yyyy", Cultura));

                    Posicionar(layers.Layer(), 40, 493, 134, 30).AlignCenter()
                        .Text("Kovalski").FontSize(25).Italic().FontColor(Texto);
                    Posicionar(layers.Layer(), 40, 535, 134, 17).AlignCenter()
                        .Text("Kovalski Tech Academy").FontSize(10);
                    Posicionar(layers.Layer(), 40, 555, 134, 14).AlignCenter()
                        .Text("COORDENAÇÃO ACADÊMICA").FontSize(6).FontColor(TextoSecundario).LetterSpacing(0.15f);

                    Posicionar(layers.Layer(), 684, 514, 115, 55)
                        .Text("LEARN\nPRACTICE\nBUILD\nYOUR FUTURE")
                        .FontSize(7).FontColor(TextoSecundario).LetterSpacing(0.2f).LineHeight(1.6f);
                });
            });
        });

        return document;
    }

    private static IContainer Posicionar(IContainer container, float x, float y, float largura, float altura)
        => container.PaddingLeft(x).PaddingTop(y).Width(largura).Height(altura);

    private static void Informacao(IContainer container, string titulo, string valor)
    {
        container.Column(column =>
        {
            column.Item().Text(titulo).FontSize(6.5f).FontColor(TextoSecundario).LetterSpacing(0.2f);
            column.Item().PaddingTop(6).Height(20).ScaleToFit().Text(valor).FontSize(10);
        });
    }

    private static string CriarArte()
    {
        // Ilustrações vetoriais inspiradas na referência, sem depender de arquivos do frontend.
        var contornoSelo = string.Join(" ", Enumerable.Range(0, 240).Select(indice =>
        {
            var angulo = indice * Math.PI * 2 / 240;
            var raio = 50 + 2.8 * Math.Cos(12 * angulo);
            return string.Create(CultureInfo.InvariantCulture,
                $"{787 + raio * Math.Cos(angulo):F2},{160 + raio * Math.Sin(angulo):F2}");
        }));

        return $$"""
            <svg xmlns="http://www.w3.org/2000/svg" width="900" height="600" viewBox="0 0 900 600">
              <defs>
                <linearGradient id="purple" x1="0" y1="0" x2="1" y2="1">
                  <stop stop-color="#8B51FF"/><stop offset="1" stop-color="#5420E8"/>
                </linearGradient>
                <linearGradient id="ribbon" x1="0" y1="0" x2="1" y2="1">
                  <stop stop-color="#332075"/><stop offset="1" stop-color="#722BFA"/>
                </linearGradient>
              </defs>
              <rect width="900" height="600" fill="#F8FAFB"/>
              <path d="M0 0H104L0 115Z" fill="#C9B6F4"/>
              <path d="M0 0H68L0 79Z" fill="url(#purple)"/>
              <path d="M768 600L900 470V600Z" fill="#D0BFF5"/>
              <path d="M820 600V514Q820 507 826 501L896 436L900 435" fill="none" stroke="#793FFF" stroke-width="0.7"/>

              <g stroke="#6C2BFF" fill="none" stroke-width="3.5" stroke-linejoin="round">
                <path d="M113 56L102 64L113 72M128 56L139 64L128 72M125 49L117 80"/>
              </g>
              <path d="M151 50V75" stroke="#A37BFF" stroke-width="0.7"/>
              <path d="M679 56H709" stroke="#6C2BFF" stroke-width="1.3"/>
              <path d="M165 280H709" stroke="#915FFF" stroke-width="0.8"/>
              <path d="M40 529H174M684 503H708" stroke="#793FFF" stroke-width="0.9"/>

              <path d="M760 195L742 238L758 234L769 247L784 207Z" fill="url(#ribbon)"/>
              <path d="M787 205L801 247L814 234L829 238L813 194Z" fill="url(#purple)"/>
              <polygon points="{{contornoSelo}}" fill="url(#purple)"/>
              <circle cx="787" cy="160" r="44" fill="#A17AFF"/>
              <circle cx="787" cy="160" r="37" fill="#44228D"/>
              <g stroke="#EEE8FF" fill="none" stroke-width="4" stroke-linejoin="round">
                <path d="M777 151L766 160L777 168M799 151L810 160L799 168M794 144L783 177"/>
              </g>

              <g stroke="#793FFF" fill="none" stroke-width="1.8" stroke-linejoin="round">
                <path d="M178 423L165 430L178 437L191 430ZM170 433V442L178 446L186 442V433M191 430V440"/>
                <circle cx="422" cy="433" r="10"/>
                <path d="M422 427V434L427 437"/>
                <rect x="562" y="423" width="19" height="19" rx="2"/>
                <path d="M562 429H581M567 420V426M577 420V426M566 433H569M572 433H575M577 433H579M566 437H569M572 437H575"/>
              </g>
              <path d="M384 418V454M529 418V454" stroke="#8C93AD" stroke-width="0.7"/>
            </svg>
            """;
    }
}
