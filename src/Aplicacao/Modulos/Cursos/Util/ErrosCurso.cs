using FluentResults;
using GeradorCertificados.Aplicacao.Compartilhado;

namespace GeradorCertificados.Aplicacao.Modulos.Cursos.Util;

public static class ErrosCurso
{
    public static Error ErroValidacao(string campo, string menssagem)
    {
        return new Error(menssagem)
        .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", campo);
    }

    public static Error ErroNaoEncontrado(string menssagem)
    {
        return new Error(menssagem)
        .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado);
    }
    public static Error Validacao(string menssagem)
    {
        return new Error(menssagem)
        .WithMetadata(nameof(TipoErro), TipoErro.Validacao);
    }

    internal static IError Conflito(string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }
}
