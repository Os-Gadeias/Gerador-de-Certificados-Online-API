using System.Collections;
using GeradorCertificados.Dominio.Compartilhado;

namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public class Certificado : EntidadeBase<Certificado>
{
    public string NomeAluno { get; set; } = string.Empty;
    public Curso Curso { get; set; } = null!;
    public StatusGeracaoCertificado Status = StatusGeracaoCertificado.NaoIniciado;

    private Certificado() { }
    public Certificado(string nomeAluno, Curso curso)
    {
        NomeAluno = nomeAluno;
        Curso = curso;
    }


    public byte[] CertifcadoPdfGerado { get; set; } = null!;

    public override void Atualizar(Certificado entidadeAtualizada)
    {
        Status = entidadeAtualizada.Status;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        throw new NotImplementedException();
    }

}
