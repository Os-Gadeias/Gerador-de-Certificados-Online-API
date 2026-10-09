export interface loginRequest {
  email: string;
  senha: string;
}
export interface LoginResponse {
  clienteId: string;
  accessToken: string;
  dataExpiracaoEmUtc: string;
}
