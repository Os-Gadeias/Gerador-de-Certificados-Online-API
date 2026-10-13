export interface createUserCommand {
  email: string;
  senha: string;
}
export interface CreateUserResponse {
  id: string;
  nomeUsuario: string;
  accessToken: string;
}
