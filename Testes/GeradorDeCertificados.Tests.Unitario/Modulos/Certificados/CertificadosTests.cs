using GeradorCertificados.Dominio.Modulos.ModuloCertificado;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Certificados;

[TestClass]
public sealed class CertificadosTests
{
    [TestMethod]
    public void DeveCriarCertificadoComStatusPendente()
    {
        // Arrange
        Certificado certificado = new(
            "Thiago Kovalski",
            new Curso());

        // Assert
        Assert.AreEqual(
            StatusCertificado.Pendente,
            certificado.StatusCertificado);
    }

    [TestMethod]
    public void DeveValidarCertificadoComDadosValidos()
    {
        // Arrange
        Certificado certificado = new(
            "Thiago Kovalski",
            new Curso());

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveRetornarErroQuandoNomeAlunoNaoForInformado()
    {
        // Arrange
        Certificado certificado = new(
            null!,
            new Curso());

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            nameof(Certificado.NomeAluno),
            erros[0].Campo);
    }

    [TestMethod]
    [DataRow("A")]
    [DataRow("")]
    public void DeveRetornarErroQuandoNomeAlunoTiverMenosDe2Caracteres(
        string nomeAluno)
    {
        // Arrange
        Certificado certificado = new(
            nomeAluno,
            new Curso());

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            nameof(Certificado.NomeAluno),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornarErroQuandoNomeAlunoTiverMaisDe200Caracteres()
    {
        // Arrange
        string nomeAluno = new string('A', 201);

        Certificado certificado = new(
            nomeAluno,
            new Curso());

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            nameof(Certificado.NomeAluno),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveAceitarNomeAlunoCom2Caracteres()
    {
        // Arrange
        Certificado certificado = new(
            "Jo",
            new Curso());

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveAceitarNomeAlunoCom200Caracteres()
    {
        // Arrange
        string nomeAluno = new string('A', 200);

        Certificado certificado = new(
            nomeAluno,
            new Curso());

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveRetornarErroQuandoCursoNaoForInformado()
    {
        // Arrange
        Certificado certificado = new(
            "Thiago Kovalski",
            null!);

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            nameof(Certificado.Curso),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornarDoisErrosQuandoNomeEcursoNaoForemInformados()
    {
        // Arrange
        Certificado certificado = new(
            null!,
            null!);

        // Act
        var erros = certificado.Validar();

        // Assert
        Assert.HasCount(2, erros);
    }

    [TestMethod]
    public void DeveAtualizarNomeAlunoECurso()
    {
        // Arrange
        Certificado certificado = new(
            "Aluno Antigo",
            new Curso());

        Curso novoCurso = new();

        Certificado certificadoAtualizado = new(
            "Aluno Novo",
            novoCurso);

        // Act
        certificado.Atualizar(certificadoAtualizado);

        // Assert
        Assert.AreEqual(
            "Aluno Novo",
            certificado.NomeAluno);

        Assert.AreSame(
            novoCurso,
            certificado.Curso);
    }

    [TestMethod]
    public void NaoDeveAlterarStatusAoAtualizarCertificado()
    {
        // Arrange
        Certificado certificado = new(
            "Aluno Antigo",
            new Curso());

        Certificado certificadoAtualizado = new(
            "Aluno Novo",
            new Curso());

        // Act
        certificado.Atualizar(certificadoAtualizado);

        // Assert
        Assert.AreEqual(
            StatusCertificado.Pendente,
            certificado.StatusCertificado);
    }
}
