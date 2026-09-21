using FluentResults;
using GeradorCertificados.Aplicacao.Consumers;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MassTransit;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos;

public sealed record SolicitarGeracaoResponse(
    List<Guid> IdCertificados,
    List<string> NomesAlunosCertificados
);

public sealed record SolicitarGeracaoCommand(
    List<string> NomesAlunos,
    Guid IdCurso
) : IRequest<Result<SolicitarGeracaoResponse>>;

public class SolicitaGeracaoCertificadoCommandHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioCertificado repositorioCertificado,
    IBus bus
    )
    : IRequestHandler<SolicitarGeracaoCommand, Result<SolicitarGeracaoResponse>>
{
    public async Task<Result<SolicitarGeracaoResponse>> Handle(SolicitarGeracaoCommand command, CancellationToken cancellationToken = default)
    {
        Curso? cursoSelecionado = await repositorioCurso.SelecionarPorIdAsync(command.IdCurso);

        //verificar se o curso existe
        if (cursoSelecionado is null)
            return Result.Fail(ErrosCurso.ErroNaoEncontrado("Curso não encontrado!"));

        //Verifica se os nomes são validos
        foreach (string nome in command.NomesAlunos)
        {
            if (nome.Length is < 3 or > 200)
                return Result.Fail(
                    $"O nome: {nome} é invádio! Deve conter entre 3 à 200 caractéres. Certifique-se de que todos os nomes estão corretos"
                    );
        }

        List<Certificado> certificados = [];

        //Cadastra os certificados no banco antes de mandar pra message
        foreach (string nome in command.NomesAlunos)
        {
            Certificado certificadoGerado = new(nome, cursoSelecionado);
            certificados.Add(certificadoGerado);
            await repositorioCertificado.CadastrarAsync(certificadoGerado);
        }

        //manda os ids dos certificados e o id do curso
        await bus.Publish(new SolicitarCertificadosMessages(
            cursoSelecionado.Id,
            certificados.Select(c => c.Id).ToList()
        ));

        //Retorna os Ids e nomes dos alunos
        return new SolicitarGeracaoResponse(
            certificados.Select(c => c.Id).ToList(),
            certificados.Select(c => c.NomeAluno).ToList()
            );
    }
}