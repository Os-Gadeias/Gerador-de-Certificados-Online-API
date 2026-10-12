using FluentResults;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificados.Aplicacao.Modulos.Usuarios.Compartilhado;

public sealed record AutenticarUsuarioDto(
    Guid ClienteId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc
);

public sealed record CadastrarUsuarioCommand(string Email, string Senha) : IRequest<Result<CadastrarUsuarioResponse>>;

public sealed record CadastrarUsuarioResponse(Guid Id, string NomeUsuario, string AccessToken);

public sealed record ObterUsuarioPorIdQuery(Guid Id) : IRequest<Result<UsuarioDto>>;
