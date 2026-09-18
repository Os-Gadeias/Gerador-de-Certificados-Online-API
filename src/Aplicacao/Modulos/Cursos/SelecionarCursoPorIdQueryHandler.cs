using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Aplicacao.Modulos.Dtos.Cursos;
using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos;

public record SelecionarCursoPorIdQuery(Guid Id) : IRequest<Result<CursoDto>>;


public class SelecionarCursoPorIdQueryHandler(IRepositorioCurso repositorio) : IRequestHandler<SelecionarCursoPorIdQuery, Result<CursoDto>>
{
    public async Task<Result<CursoDto>> Handle(SelecionarCursoPorIdQuery request, CancellationToken cancellationToken)
    {
        Curso? curso = await repositorio.SelecionarPorIdAsync(request.Id);

        if (curso is null)
        {
            Result.Fail(ErrosCurso.ErroNaoEncontrado(
                $"O \"curso\" com o Id {request.Id} não foi encontrado"));
        }

        return Result.Ok(new CursoDto(
            curso!.Id,
            curso.Nome,
            curso.Descricao,
            curso.CargaHoraria,
            curso.DataConclusao
        ));
    }
}