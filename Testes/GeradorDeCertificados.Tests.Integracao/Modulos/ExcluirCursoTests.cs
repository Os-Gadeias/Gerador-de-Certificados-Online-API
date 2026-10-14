using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorCertificados.Infraestrutura.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Tests.Integracao.Modulos;

[TestClass]
public sealed class ExcluirCursoTests
{
    [TestMethod]
    public async Task Deve_ExcluirCursoDoUsuarioEApenasAqueleCursoDoBanco()
    {
        // Arrange
        string databaseName = Guid.CreateVersion7().ToString();
        Guid usuarioId = Guid.CreateVersion7();
        Guid outroUsuarioId = Guid.CreateVersion7();
        DbContextOptions<GeradorCertificadosDbContext> options =
            new DbContextOptionsBuilder<GeradorCertificadosDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

        Curso cursoDoUsuario = new(
            "Curso do usuário",
            null,
            20,
            DateTime.Today.AddDays(10));
        Curso cursoDeOutroUsuario = new(
            "Curso de outro usuário",
            null,
            30,
            DateTime.Today.AddDays(20));

        await using GeradorCertificadosDbContext contextoUsuario = new(
            options,
            new ProvedorDeUsuarioFake(usuarioId));
        IRepositorioCurso repositorioUsuario = new RepositorioCursoEmOrm(
            contextoUsuario,
            new ProvedorDeUsuarioFake(usuarioId));
        await repositorioUsuario.CadastrarAsync(cursoDoUsuario);

        await using GeradorCertificadosDbContext contextoOutroUsuario = new(
            options,
            new ProvedorDeUsuarioFake(outroUsuarioId));
        IRepositorioCurso repositorioOutroUsuario = new RepositorioCursoEmOrm(
            contextoOutroUsuario,
            new ProvedorDeUsuarioFake(outroUsuarioId));
        await repositorioOutroUsuario.CadastrarAsync(cursoDeOutroUsuario);

        ExcluirCursoPorIdCommandHandler handler = new(repositorioUsuario);

        // Act
        var resultado = await handler.Handle(
            new ExcluirCursoPorIdCommand(cursoDoUsuario.Id),
            CancellationToken.None);
        bool tentouExcluirCursoDeOutroUsuario = await repositorioUsuario.ExcluirAsync(
            cursoDeOutroUsuario.Id);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(resultado.Value);
        Assert.IsFalse(tentouExcluirCursoDeOutroUsuario);

        await using GeradorCertificadosDbContext contextoVerificacao = new(
            options,
            new ProvedorDeUsuarioFake(usuarioId));
        Assert.IsFalse(await contextoVerificacao.Cursos
            .IgnoreQueryFilters()
            .AnyAsync(curso => curso.Id == cursoDoUsuario.Id));
        Assert.IsTrue(await contextoVerificacao.Cursos
            .IgnoreQueryFilters()
            .AnyAsync(curso => curso.Id == cursoDeOutroUsuario.Id));
    }

    private sealed class ProvedorDeUsuarioFake(Guid usuarioId) : IProvedorDeUsuario
    {
        public Guid? Id => usuarioId;

        public string? Email => null;

        public bool EstaAutenticado => true;

        public bool PossuiTipo(TipoUsuario tipoUsuario) => false;

        public void DefinirUsuario(Guid id)
        {
            throw new NotSupportedException();
        }
    }
}
