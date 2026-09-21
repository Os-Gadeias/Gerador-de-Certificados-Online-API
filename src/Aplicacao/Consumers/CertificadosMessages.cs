namespace GeradorCertificados.Aplicacao.Consumers;

public record SolicitarCertificadosMessages(
    List<string> NomeAlunos,
    Guid CursoId
);
