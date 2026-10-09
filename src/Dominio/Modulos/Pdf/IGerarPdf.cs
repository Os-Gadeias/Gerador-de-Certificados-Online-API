using GeradorCertificados.Dominio.Modulos.ModuloCertificado;

namespace GeradorCertificados.Dominio.Modulos.Pdf;

public interface IGerarPdf
{
    byte[] Gerar(Certificado certificado);
}