using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Dtos.Cursos;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos;

public sealed record SelecionarTodosCursosQuery : IRequest<Result<List<CursoDto>>>;

public sealed class SelecionarTodosCursosQueryHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<SelecionarTodosCursosQuery, Result<List<CursoDto>>>
{
    public async Task<Result<List<CursoDto>>> Handle(
        SelecionarTodosCursosQuery query,
        CancellationToken cancellationToken)
    {
        List<Curso> cursos = await repositorioCurso.SelecionarTodosAsync(cancellationToken);

        List<CursoDto> cursosDto = cursos
            .Select(curso => new CursoDto(
                curso.Id,
                curso.Nome,
                curso.Descricao,
                curso.CargaHoraria,
                curso.DataConclusao,
                curso.Certificados?.Count))
            .ToList();

        return Result.Ok(cursosDto);
    }
}
