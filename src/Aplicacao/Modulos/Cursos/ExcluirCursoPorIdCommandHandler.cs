using FluentResults;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos;

public sealed record ExcluirCursoPorIdCommand(Guid Id) : IRequest<Result<bool>>;

public sealed class ExcluirCursoPorIdCommandHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<ExcluirCursoPorIdCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirCursoPorIdCommand command,
        CancellationToken cancellationToken)
    {
        bool excluido = await repositorioCurso.ExcluirAsync(command.Id, cancellationToken);

        return Result.Ok(excluido);
    }
}
