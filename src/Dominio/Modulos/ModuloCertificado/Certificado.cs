using GeradorCertificados.Dominio.Compartilhado;

namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public class Certificado : EntidadeBase<Certificado>
{
    public string NomeAluno { get; set; } = string.Empty;
    public Curso Curso { get; set; } = null!;
    public StatusCertificado StatusCertificado { get; private set; } = StatusCertificado.Pendente;
    public override void Atualizar(Certificado entidadeAtualizada)
    {
        NomeAluno = entidadeAtualizada.NomeAluno;
        Curso = entidadeAtualizada.Curso;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (NomeAluno is null)
            erros.Add(new ErroValidacao(nameof(NomeAluno), "O campo \"Nome Aluno \" é obrigatório!"));

        else if (NomeAluno.Length is < 2 or > 200)
            erros.Add(new ErroValidacao(nameof(NomeAluno),
                "O campo \"Nome Aluno \" deve conter entre 2 à 200 caracteres!"));

        if (Curso is null)
            erros.Add(new ErroValidacao(nameof(Curso), "O campo \"Curso\" é obrigatório!"));

        return erros;
    }

}
