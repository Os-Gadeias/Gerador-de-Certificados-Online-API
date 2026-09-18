using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;

public class Curso : EntidadeBase<Curso>, IEntidadeDeUsuario
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public DateTime DataConclusao { get; set; } = DateTime.MinValue;
    public Guid UsuarioId { get; set; }

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
        Nome = entidadeAtualizada.Nome;
        Descricao = entidadeAtualizada.Descricao;
        CargaHoraria = entidadeAtualizada.CargaHoraria;
        DataConclusao = entidadeAtualizada.DataConclusao;
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