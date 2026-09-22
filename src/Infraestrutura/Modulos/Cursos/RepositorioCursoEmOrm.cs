using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificados.Infraestrutura.Modulos.Cursos;

public class RepositorioCursoEmOrm(GeradorCertificadosDbContext dbContext, IProvedorDeUsuario provedorDeUsuario)
     : RepositorioBaseEmOrm<Curso>(dbContext, provedorDeUsuario), IRepositorioCurso
{
     public override async Task<Curso?> SelecionarPorIdAsync(Guid idSelecionado, CancellationToken cancellationToken = default)
     {
          return await registros
              .Include(c => c.Certificados)
              .FirstOrDefaultAsync(c => c.Id == idSelecionado, cancellationToken);
     }

}