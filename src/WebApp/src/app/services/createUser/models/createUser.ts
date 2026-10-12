export interface createUserCommand {
  email: string;
  senha: string;
}
export interface CreateUserResponse {
  usuarioId: string;
  nomeUsuario: string;
  acessToken: string;
}
