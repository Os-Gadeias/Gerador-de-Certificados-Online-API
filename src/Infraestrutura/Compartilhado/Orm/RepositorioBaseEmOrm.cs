using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificados.Infraestrutura.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrm<T>(GeradorCertificadosDbContext dbContext,
     IProvedorDeUsuario provedorDeUsuario)
        where T : EntidadeBase<T>
{
    protected readonly DbSet<T> registros = dbContext.Set<T>();

    public async Task CadastrarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        registros.Add(entidade);

        await SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<bool> EditarAsync(
        Guid id,
        T entidadeAtualizada,
        CancellationToken cancellationToken = default
    )
    {
        T? registroSelecionado = await SelecionarPorIdAsync(id, cancellationToken);

        if (registroSelecionado == null)
            return false;

        registroSelecionado.Atualizar(entidadeAtualizada);

        await SalvarAlteracoesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        T? registroSelecionado = await SelecionarPorIdAsync(id, cancellationToken);

        if (registroSelecionado == null)
            return false;

        registros.Remove(registroSelecionado);

        await SalvarAlteracoesAsync(cancellationToken);

        return true;
    }

    public virtual async Task<T?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return await registros.SingleOrDefaultAsync(c => c.Id == idSelecionado, cancellationToken);
    }

    public virtual async Task<List<T>> SelecionarTodosAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await registros.ToListAsync(cancellationToken);
    }

    protected async Task SalvarAlteracoesAsync(
    CancellationToken cancellationToken = default)
    {
        try
        {
            Guid? usuarioId = provedorDeUsuario?.Id;

            if (!usuarioId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Não é possível salvar entidades do usuário sem estar autenticado."
                );
            }

            foreach (var entry in dbContext.ChangeTracker.Entries<IEntidadeDeUsuario>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:

                        if (entry.Entity.UsuarioId == Guid.Empty)
                        {
                            entry.Property(nameof(IEntidadeDeUsuario.UsuarioId))
                                .CurrentValue = usuarioId.Value;
                        }
                        else if (entry.Entity.UsuarioId != usuarioId.Value)
                        {
                            throw new UnauthorizedAccessException(
                                "Tentativa de criar entidade para outro usuário."
                            );
                        }

                        break;

                    case EntityState.Modified:

                        Guid usuarioOriginalId =
                            entry.Property(nameof(IEntidadeDeUsuario.UsuarioId))
                                .OriginalValue is Guid original
                                ? original
                                : Guid.Empty;

                        Guid usuarioAtualId =
                            entry.Property(nameof(IEntidadeDeUsuario.UsuarioId))
                                .CurrentValue is Guid atual
                                ? atual
                                : Guid.Empty;

                        if (usuarioOriginalId != usuarioAtualId)
                        {
                            throw new UnauthorizedAccessException(
                                "Não é permitido alterar o usuário de uma entidade."
                            );
                        }

                        if (usuarioOriginalId != usuarioId.Value)
                        {
                            throw new UnauthorizedAccessException(
                                "Tentativa de modificar entidade de outro usuário."
                            );
                        }

                        break;

                    case EntityState.Deleted:

                        Guid usuarioOriginal =
                            entry.Property(nameof(IEntidadeDeUsuario.UsuarioId))
                                .OriginalValue is Guid originalId
                                ? originalId
                                : Guid.Empty;

                        if (usuarioOriginal != usuarioId.Value)
                        {
                            throw new UnauthorizedAccessException(
                                "Tentativa de excluir entidade de outro usuário."
                            );
                        }

                        break;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            dbContext.ChangeTracker.Clear();

            throw new ConflitoDePersistenciaException(
                "Ocorreu um erro ao persistir os dados.",
                ex
            );
        }
    }
}
