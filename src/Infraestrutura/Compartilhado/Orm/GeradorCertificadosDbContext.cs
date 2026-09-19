using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GeradorCertificados.Dominio.Compartilhado.Auth;

namespace GeradorCertificados.Infraestrutura.Compartilhado.Orm;

public sealed class GeradorCertificadosDbContext(
    DbContextOptions<GeradorCertificadosDbContext> options,
    IProvedorDeUsuario? provedorDeUsuario = null
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Curso> Cursos => Set<Curso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorCertificadosDbContext).Assembly);

        Guid? userId = provedorDeUsuario?.Id;

        modelBuilder.Entity<Curso>()
        .HasQueryFilter(c => c.UsuarioId == provedorDeUsuario!.Id);

    }
}
