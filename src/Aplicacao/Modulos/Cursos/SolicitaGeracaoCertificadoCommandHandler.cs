using System.Reflection.Metadata.Ecma335;
using FluentResults;
using GeradorCertificados.Aplicacao.Compartilhado;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MassTransit;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos;

public sealed record SolicitarGeracaoCommand(
    List<string> NomesAlunos,
    Guid IdCurso
) : IRequest<Result>;

public class SolicitaGeracaoCertificadoCommandHandler(
    IRepositorioCurso repositorioCurso,
    IBus bus
    )
    : IRequestHandler<SolicitarGeracaoCommand, Result>
{
    public async Task<Result> Handle(SolicitarGeracaoCommand command, CancellationToken cancellationToken = default)
    {
        Curso? cursoSelecionado = await repositorioCurso.SelecionarPorIdAsync(command.IdCurso);

        //verificar se o curso existe
        if (cursoSelecionado is null)
            return Result.Fail(ErrosCurso.ErroNaoEncontrado("Curso não encontrado!"));

        List<string> alunosComNomeValidos = [];

        foreach (string nome in command.NomesAlunos)
        {
            if (nome.Length is < 2 or > 200)
                continue;

            alunosComNomeValidos.Add(nome);
        }

        await bus.Publish(cursoSelecionado);

        return Result.Ok();
    }
}