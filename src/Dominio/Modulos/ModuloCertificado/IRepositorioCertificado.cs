using GeradorCertificados.Dominio.Compartilhado;

namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public interface IRepositorioCertificado : IRepositorio<Certificado>
{
    Task<List<Certificado>> SelecionarPorCursoAsync(
        Guid idCurso,
        CancellationToken cancellationToken = default
    );
}
