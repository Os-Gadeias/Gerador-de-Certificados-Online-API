using FluentResults;
using GeradorCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Certificados;

public sealed record SolicitarDownloadDeCertificadosCommand(
    Guid IdCurso
) : IRequest<Result<DownloadCertificadosResponse>>;

public class DownloadDeCertificadosHandler(
    IRepositorioCurso repositorioCurso
) :
    IRequestHandler<SolicitarDownloadDeCertificadosCommand, Result<DownloadCertificadosResponse>>
{
    public async Task<Result<DownloadCertificadosResponse>> Handle(
        SolicitarDownloadDeCertificadosCommand request,
        CancellationToken cancellationToken)
    {
        Curso? cursoSelecionado = await repositorioCurso.SelecionarPorIdAsync(request.IdCurso);

        //verificar se o curso existe
        if (cursoSelecionado is null)
            return Result.Fail(ErrosCurso.ErroNaoEncontrado("Curso não encontrado!"));

        if (string.IsNullOrWhiteSpace(cursoSelecionado.CaminhoZip))
            return Result.Fail(ErrosCurso.ErroNaoEncontrado("O ZIP ainda não encontrado."));

        if (!File.Exists(cursoSelecionado.CaminhoZip))
            return Result.Fail(ErrosCurso.ErroNaoEncontrado("O arquivo ZIP não foi encontrado."));

        var bytes = await File.ReadAllBytesAsync(cursoSelecionado.CaminhoZip);

        return Result.Ok(new DownloadCertificadosResponse(
            Arquivo: bytes,
            NomeArquivo: $"certificados-{cursoSelecionado.Id}.zip"
        ));

    }
}