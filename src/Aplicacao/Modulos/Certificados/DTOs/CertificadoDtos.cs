namespace GeradorCertificados.Aplicacao.Modulos.Certificados.DTOs;

public sealed record DownloadCertificadosResponse(
    byte[] Arquivo,
    string NomeArquivo
);