namespace GeradorCertificados.Aplicacao.Modulos.Dtos.Cursos;

public record CursoDto(Guid Id, string Nome, string? Descricao, int CargaHoraria, DateTime DataConclusao);
