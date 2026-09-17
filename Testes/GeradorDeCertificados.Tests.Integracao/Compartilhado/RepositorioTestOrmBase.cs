using GeradorCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Tests.Integracao.Compartilhado;

public abstract class RepositorioTestOrmBase
{
    protected GeradorCertificadosDbContext dbContext = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext();
    }
    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private static GeradorCertificadosDbContext CriarDbContext()
    {
        DbContextOptions<GeradorCertificadosDbContext> options =
            new DbContextOptionsBuilder<GeradorCertificadosDbContext>()
                .UseInMemoryDatabase("GeradorDeProvasTestDB_Memory")
                .Options;

        return new GeradorCertificadosDbContext(options);
    }
}
