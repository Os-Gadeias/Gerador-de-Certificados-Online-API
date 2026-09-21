namespace GeradorCertificados.WebApi.Modulos.Certificados;

public sealed record SolicitarGeracaoDeCertificadosCommand(
    List<string> NomesAlunos
);
public sealed record SolicitarGeracaoDeCertificadosResponse(
    List<Guid> IdsCertificados,
    List<string> NomesAlunos
); 