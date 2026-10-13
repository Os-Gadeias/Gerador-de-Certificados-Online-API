import { HttpErrorResponse } from '@angular/common/http';

interface ApiError {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errors?: Record<string, string[]>;
  traceId?: string;
}

export function extrairMensagemErro(erro: HttpErrorResponse): string {
  const apiError = erro.error as ApiError;

  if (apiError.errors) {
    const mensagens = Object.values(apiError.errors).flat();

    if (mensagens.length > 0) {
      return mensagens[0];
    }
  }

  return apiError.detail ?? apiError.title ?? 'Ocorreu um erro inesperado.';
}
