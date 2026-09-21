namespace GeradorCertificados.Dominio.Modulos.ModuloCertificado;

public enum StatusGeracaoCertificado
{
    NaoIniciado,
    Pendente,
    GerandoCertificados,
    GerandoZip,
    Concluído,
    Falha
}