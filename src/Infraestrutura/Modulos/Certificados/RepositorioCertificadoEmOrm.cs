using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificados.Infraestrutura.Modulos.Certificados;

public class RepositorioCertificadoEmOrm(
    GeradorCertificadosDbContext dbContext, IProvedorDeUsuario provedorDeUsuario
    ) : RepositorioBaseEmOrm<Certificado>(dbContext, provedorDeUsuario), IRepositorioCertificado
{
    public async Task<List<Certificado>> SelecionarPorCursoAsync(
        Guid idCurso,
        CancellationToken cancellationToken = default)
    {
        //selecione todos os Certificados pelo Id do curso, onde o idCurso seja igual a chave estrangeira "CursoId" na tabela
        return await registros
            .Where(certificado => EF.Property<Guid>(certificado, "CursoId") == idCurso)
            .ToListAsync();
    }
}
