using GeradorCertificados.Aplicacao.Modulos.Cursos;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
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

        var certificadoDoUsuario = new Certificado("Aluno do usuário", cursoDoUsuario);
        var certificadoDeOutroUsuario = new Certificado("Aluno de outro usuário", cursoDeOutroUsuario);
        cursoDoUsuario.AddCertificados([certificadoDoUsuario]);
        cursoDeOutroUsuario.AddCertificados([certificadoDeOutroUsuario]);

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
        Assert.IsFalse(await contextoVerificacao.Set<Certificado>()
            .AnyAsync(c => c.Id == certificadoDoUsuario.Id));
        Assert.IsTrue(await contextoVerificacao.Set<Certificado>()
            .AnyAsync(c => c.Id == certificadoDeOutroUsuario.Id));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Deve_ExcluirCursoComCertificados_SemExcluirOutrosCursos(bool limparRastreamento)
    {
        var usuario = new ProvedorDeUsuarioFake(Guid.NewGuid());
        var options = new DbContextOptionsBuilder<GeradorCertificadosDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var contexto = new GeradorCertificadosDbContext(options, usuario);
        IRepositorioCurso repositorio = new RepositorioCursoEmOrm(contexto, usuario);
        var curso = new Curso("Curso a excluir", null, 20, DateTime.Today);
        var outroCurso = new Curso("Curso preservado", null, 20, DateTime.Today);
        var certificadoUm = new Certificado("Aluno um", curso);
        var certificadoDois = new Certificado("Aluno dois", curso);
        var certificadoPreservado = new Certificado("Aluno preservado", outroCurso);
        curso.AddCertificados([certificadoUm, certificadoDois]);
        outroCurso.AddCertificados([certificadoPreservado]);
        await repositorio.CadastrarAsync(curso);
        await repositorio.CadastrarAsync(outroCurso);
        if (limparRastreamento)
            contexto.ChangeTracker.Clear();

        Assert.IsTrue(await repositorio.ExcluirAsync(curso.Id));

        await using var verificacao = new GeradorCertificadosDbContext(options, usuario);
        Assert.IsFalse(await verificacao.Cursos.AnyAsync(c => c.Id == curso.Id));
        Assert.IsTrue(await verificacao.Cursos.AnyAsync(c => c.Id == outroCurso.Id));
        var certificadosRestantes = await verificacao.Set<Certificado>().ToListAsync();
        Assert.HasCount(1, certificadosRestantes);
        Assert.AreEqual(certificadoPreservado.Id, certificadosRestantes[0].Id);
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
