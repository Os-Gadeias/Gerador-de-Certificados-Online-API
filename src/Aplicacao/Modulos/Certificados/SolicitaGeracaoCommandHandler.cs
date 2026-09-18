using System.Runtime.ConstrainedExecution;
using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Certificados;

public sealed record SolicitarGeracaoCommand(
    string NomeAluno,
    Guid IdCurso
) : IRequest<Result<Guid>>;

public class SolicitaGeracaoCommandHandler(
    IRepositorioCertificado repositorioCertificado
    )
    : IRequestHandler<SolicitarGeracaoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SolicitarGeracaoCommand request, CancellationToken cancellationToken)
    {
        Curso curso = new("Academia do Programador", "Programação", 600, DateTime.Now.AddDays(20));

        Certificado certificado = new(request.NomeAluno, curso);

        var erros = certificado.Validar();

        if (erros.Count > 0)
            Result.Fail<Guid>
                (ErrosCertificado.ErroValidacao(erros.First().Campo, erros.First().Mensagem));

        await repositorioCertificado.CadastrarAsync(certificado);

        return Result.Ok(certificado.Id);

    }
}