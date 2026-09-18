using GeradorCertificados.Aplicacao.Modulos.Certificados;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using Moq;

namespace GeradorDeCertificados.Tests.Unitario.Modulos.Certificados.Handlers;

[TestClass]
public class SolicitaGeracaoHandlerTest
{
    [TestMethod]
    public async Task NaoDeve_RetornarErro_SolicitarGeracao_De_CertificadoValido()
    {
        Mock<IRepositorioCertificado> repositorioCertificado = new();

        Certificado certificadoCadastrado = null!;

        repositorioCertificado.Setup(r =>
            r.CadastrarAsync(It.IsAny<Certificado>(),
            It.IsAny<CancellationToken>()))
            .Callback<Certificado, CancellationToken>((r, c) => certificadoCadastrado = r);

        SolicitaGeracaoCommandHandler handler = new(repositorioCertificado.Object);

        var resultado = await handler.Handle(new("Thiago Kovalski", Guid.CreateVersion7()), default);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(certificadoCadastrado);
        Assert.AreEqual("Thiago Kovalski", certificadoCadastrado.NomeAluno);
    }
}
