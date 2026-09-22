using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Aplicacao.Modulos.Dtos.Cursos;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Certificados;

public record SelecionarCursoPorIDQuery(Guid Id) : IRequest<Result<SelecionarCursoResponse>>;
public class VerificarStatusCursoQueryHandler(IRepositorioCurso repositorioCurso)
    : IRequestHandler<SelecionarCursoPorIDQuery, Result<SelecionarCursoResponse>>
{
    public async Task<Result<SelecionarCursoResponse>> Handle(SelecionarCursoPorIDQuery request
    , CancellationToken cancellationToken = default)
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(request.Id);

        if (curso is null)
        {
            return Result.Fail(ErrosCurso.ErroNaoEncontrado(
                $"O \"Curso\" com o Id {request.Id} não foi encontrado"
            ));
        }

        return Result.Ok(new SelecionarCursoResponse(
            curso.Status
        ));
    }
}