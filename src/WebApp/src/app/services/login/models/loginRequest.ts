export interface loginRequest {
  email: string;
  senha: string;
}
export interface LoginResponse {
  clienteId: string;
  token: string;
  DataExpiracaoEmUtc: string;
}
