using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificados.Infraestrutura.Modulos.Cursos;

public class RepositorioCursoEmOrm(GeradorCertificadosDbContext dbContext, IProvedorDeUsuario provedorDeUsuario)
     : RepositorioBaseEmOrm<Curso>(dbContext, provedorDeUsuario), IRepositorioCurso
{
     private readonly DbSet<Certificado> certificados = dbContext.Set<Certificado>();

     public override async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
     {
          // A consulta respeita o filtro do usuário e carrega os certificados vinculados.
          var curso = await SelecionarPorIdAsync(id, cancellationToken);
          if (curso is null)
               return false;

          // O vínculo é obrigatório e usa Restrict: exclua os dependentes explicitamente.
          if (curso.Certificados is not null)
               certificados.RemoveRange(curso.Certificados);

          registros.Remove(curso);
          await SalvarAlteracoesAsync(cancellationToken);
          return true;
     }

     public override async Task<Curso?> SelecionarPorIdAsync(Guid idSelecionado, CancellationToken cancellationToken = default)
     {
          return await registros
              .Include(c => c.Certificados)
              .FirstOrDefaultAsync(c => c.Id == idSelecionado, cancellationToken);
     }
    public override async Task<List<Curso>> SelecionarTodosAsync(CancellationToken cancellationToken = default)
    {
        return await registros.Include(c => c.Certificados).ToListAsync();
    }
}
