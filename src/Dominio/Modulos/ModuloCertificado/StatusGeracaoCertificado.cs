namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public enum StatusGeracaoCertificado
{
    NaoIniciado,
    Pendente,
    GerandoCertificado,
    GerandoZip,
    Concluído,
    Falha
}