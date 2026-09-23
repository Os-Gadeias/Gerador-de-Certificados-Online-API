using FluentResults;
using GeradorCertificados.Aplicacao.Compartilhado;
using GeradorCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

public record SelecionaCertificadoCursoPorIdQuery(Guid Id)
    : IRequest<Result<SolicitarListaCertificadoResponse>>;

public class SolicitaListaCertificadosHandler(IRepositorioCurso repositorioCurso)
    : IRequestHandler<SelecionaCertificadoCursoPorIdQuery, Result<SolicitarListaCertificadoResponse>>
{
    public async Task<Result<SolicitarListaCertificadoResponse>> Handle(
        SelecionaCertificadoCursoPorIdQuery request, CancellationToken cancellationToken = default)
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(request.Id);

        if (curso is null)
        {
            return Result.Fail(ErrosCurso.ErroNaoEncontrado(
                $"O \"Curso\" com o Id {request.Id} não foi encontrado"
            ));
        }

        if (curso.Certificados is null)
            return Result.Fail(ErrosCurso.Conflito("Curso não possui certificados atrelados!"));

        return new SolicitarListaCertificadoResponse(curso.Certificados.Select(c => new ListarCertificadoDto(
            c.NomeAluno,
            c.Curso.Nome,
            c.Curso.Descricao,
            c.Curso.CargaHoraria,
            c.Curso.DataConclusao,
            c.Status)).ToList());
    }
}