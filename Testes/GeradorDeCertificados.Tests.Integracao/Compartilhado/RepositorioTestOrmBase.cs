using GeradorCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using GeradorCertificados.Infraestrutura.Compartilhado.Auth;

namespace GeradorDeCertificados.Tests.Integracao.Compartilhado;

public abstract class RepositorioTestOrmBase
{
    protected GeradorCertificadosDbContext dbContext = null!;
    //Configuracao do UserManager do EntityFramework
    private UserManager<IdentityUser<Guid>> userManager = null!;
    protected GerenciadorDeIdentidade gerenciadorDeIdentidade = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext();

        ServiceCollection services = new();

        services.AddLogging();

        services.AddDataProtection();
        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<GeradorCertificadosDbContext>();

        services.AddSingleton(dbContext);

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        UserManager<IdentityUser<Guid>> userManager =
            serviceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

        gerenciadorDeIdentidade = new(userManager);

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
