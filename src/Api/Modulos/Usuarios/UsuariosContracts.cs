namespace GeradorCertificados.WebApi.Modulos.Usuarios;

public sealed record CadastrarUsuarioRequest(
    string Email,
    string Senha
);
public sealed record CadastrarUsuarioResponse(
    Guid UsuarioId
);
public sealed record ObterUsuarioResponse(
    Guid Id,
    string Email
);
public sealed record LogarUsuarioRequest(
    string Email,
    string Senha
);

