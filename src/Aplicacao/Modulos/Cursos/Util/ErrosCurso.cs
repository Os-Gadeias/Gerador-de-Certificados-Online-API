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
}