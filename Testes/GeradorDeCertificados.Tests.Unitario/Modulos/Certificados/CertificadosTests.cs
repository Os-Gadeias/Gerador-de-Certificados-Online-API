using GeradorCertificados.Dominio.Modulos.ModuloCertificado;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Certificados;

[TestClass]
public sealed class CertificadosTests()
{
    [TestClass]
    public class CertificadoTests
    {
        [TestMethod]
        public void DeveCriarCertificadoComStatusPendente()
        {
            // Arrange
            Certificado certificado = new();

            // Assert
            Assert.AreEqual(
                StatusCertificado.Pendente,
                certificado.StatusCertificado);
        }

        [TestMethod]
        public void DeveValidarCertificadoComDadosValidos()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = "Thiago Kovalski",
                Curso = new Curso()
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void DeveRetornarErroQuandoNomeAlunoNaoForInformado()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = null!,
                Curso = new Curso()
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(nameof(Certificado.NomeAluno), erros[0].Campo);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("")]
        public void DeveRetornarErroQuandoNomeAlunoTiverMenosDe2Caracteres(
            string nomeAluno)
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = nomeAluno,
                Curso = new Curso()
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(nameof(Certificado.NomeAluno), erros[0].Campo);
        }

        [TestMethod]
        public void DeveRetornarErroQuandoNomeAlunoTiverMaisDe200Caracteres()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = new string('A', 201),
                Curso = new Curso()
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(nameof(Certificado.NomeAluno), erros[0].Campo);
        }

        [TestMethod]
        public void DeveAceitarNomeAlunoCom2Caracteres()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = "Jo",
                Curso = new Curso()
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void DeveAceitarNomeAlunoCom200Caracteres()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = new string('A', 200),
                Curso = new Curso()
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void DeveRetornarErroQuandoCursoNaoForInformado()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = "Thiago Kovalski",
                Curso = null!
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(nameof(Certificado.Curso), erros[0].Campo);
        }

        [TestMethod]
        public void DeveRetornarDoisErrosQuandoNomeEcursoNaoForemInformados()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = null!,
                Curso = null!
            };

            // Act
            var erros = certificado.Validar();

            // Assert
            Assert.HasCount(2, erros);
        }

        [TestMethod]
        public void DeveAtualizarNomeAlunoECurso()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = "Aluno Antigo",
                Curso = new Curso()
            };

            Curso novoCurso = new Curso();

            Certificado certificadoAtualizado = new()
            {
                NomeAluno = "Aluno Novo",
                Curso = novoCurso
            };

            // Act
            certificado.Atualizar(certificadoAtualizado);

            // Assert
            Assert.AreEqual("Aluno Novo", certificado.NomeAluno);
            Assert.AreSame(novoCurso, certificado.Curso);
        }

        [TestMethod]
        public void NaoDeveAlterarStatusAoAtualizarCertificado()
        {
            // Arrange
            Certificado certificado = new()
            {
                NomeAluno = "Aluno Antigo",
                Curso = new Curso()
            };

            Certificado certificadoAtualizado = new()
            {
                NomeAluno = "Aluno Novo",
                Curso = new Curso()
            };

            // Act
            certificado.Atualizar(certificadoAtualizado);

            // Assert
            Assert.AreEqual(
                StatusCertificado.Pendente,
                certificado.StatusCertificado);
        }

    }
}