using GeradorCertificados.Dominio.Modulos.ModuloCertificado;

namespace GeradorCertificados.Aplicacao.Consumers;

public record SolicitarCertificadosMessages(
    Guid IdCurso,
    List<Guid> IdsCertificados
);
