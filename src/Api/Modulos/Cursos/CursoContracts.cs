public record CadastrarCursoRequest(string Nome, string? Descricao, int CargaHoraria, DateTime DataConclusao);
public record CadastrarCursoResponce(Guid Id);