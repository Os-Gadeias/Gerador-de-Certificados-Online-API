using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
public class Curso : EntidadeBase<Curso>, IEntidadeDeUsuario
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public DateTime DataConclusao { get; set; } = DateTime.MinValue;
    public Guid UsuarioId { get; set; }
    public List<Certificado> Certificados { get; set; }
    public StatusCurso Status { get; private set; } = StatusCurso.Disponivel;
    public string? CaminhoZip { get; set; }
    public Curso() { }

    public Curso(string nome, string? descrocao, int cargaHoraria, DateTime dataConclusao)
    {
        Nome = nome;
        Descricao = descrocao;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public override void Atualizar(Curso entidadeAtualizada)
    {
        Status = entidadeAtualizada.Status;
        CaminhoZip = entidadeAtualizada.CaminhoZip;
    }
    public void AddCertificados(List<Certificado> certificados)
    {
        Certificados = certificados;
    }

    public void AlterarParaGerandoCertificados()
    {
        Status = StatusCurso.GerandoCertificados;
    }
    public void AlterarParaDisponivel()
    {
        Status = StatusCurso.Disponivel;
    }
    public void AlterarParaFalha()
    {
        Status = StatusCurso.Falha;
    }
    public void AdicionarCaminhoDoZip(string caminhoZip)
    {
        CaminhoZip = caminhoZip;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (Nome.Length is < 2 or > 200)
        {
            erros.Add(new ErroValidacao(
                nameof(Nome),
                "O \"Nome\" deve conter entre 2 e 200 caracteres."
            ));
        }

        if (Descricao?.Length > 500)
        {
            erros.Add(new ErroValidacao(
                nameof(Descricao),
                "O campo \"Descricao\" deve conter no maximo 500 caracteres"
            ));
        }

        if (CargaHoraria <= 0)
        {
            erros.Add(new ErroValidacao(
                nameof(CargaHoraria),
                "O campo \"Carga Horaria\" "
            ));
        }

        if (DataConclusao == DateTime.MinValue)
        {
            erros.Add(new ErroValidacao(
                nameof(DataConclusao),
                "O campo \"Data de Conclusão\" deve ser  preenchida"
            ));
        }

        return erros;
    }
}