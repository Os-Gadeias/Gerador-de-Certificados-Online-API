using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos;

public record CadastrarCursoCommand(string Nome, string? Descricao, int CargaHoraria, DateTime DataConclusao) : IRequest<Result<Guid>>;
public class CadastrarCursoCommandHandler(IRepositorioCurso repositorioCurso) : IRequestHandler<CadastrarCursoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CadastrarCursoCommand command, CancellationToken cancellationToken = default)
    {
        Curso curso = new(
            command.Nome,
            command.Descricao,
            command.CargaHoraria,
            command.DataConclusao
        );

        var erros = curso.Validar();

        if (erros.Count >= 1)
        {
            return Result.Fail(ErrosCurso.ErroValidacao(erros.First().Campo, erros.First().Mensagem));
        }

        await repositorioCurso.CadastrarAsync(curso);

        return Result.Ok(curso.Id);
    }
}