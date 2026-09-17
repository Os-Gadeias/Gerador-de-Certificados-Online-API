using GeradorCertificados.Dominio.Compartilhado;

public class Curso : EntidadeBase<Curso>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descrocao { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public DateTime DataConclusao { get; set; } = DateTime.MinValue;


    public Curso() { }

    public Curso(string nome, string? descrocao, int cargaHoraria, DateTime dataConclusao)
    {
        Nome = nome;
        Descrocao = descrocao;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public override void Atualizar(Curso entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Descrocao = entidadeAtualizada.Descrocao;
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

        if (Descrocao?.Length > 500)
        {
            erros.Add(new ErroValidacao(
                nameof(Descrocao),
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