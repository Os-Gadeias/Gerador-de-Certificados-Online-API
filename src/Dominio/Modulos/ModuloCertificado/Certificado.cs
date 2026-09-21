using GeradorCertificados.Dominio.Compartilhado;

namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public class Certificado : EntidadeBase<Certificado>
{
    public string NomeAluno { get; set; } = string.Empty;
    public Curso Curso { get; set; } = null!;
    public override void Atualizar(Certificado entidadeAtualizada)
    {
        NomeAluno = entidadeAtualizada.NomeAluno;
        Curso = entidadeAtualizada.Curso;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        throw new NotImplementedException();
    }

}
