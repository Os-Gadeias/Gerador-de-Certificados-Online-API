import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ServiceStorage } from '../../../auth/serivceStorage';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(ServiceStorage).get('token');
  const requestUrl = new URL(
    req.url,
    typeof window === 'undefined' ? 'https://localhost:7094' : window.location.origin,
  );
  const isTrustedApiRequest =
    requestUrl.origin === 'https://localhost:7094' &&
    requestUrl.pathname.startsWith('/api/') &&
    !/^\/api\/auth\/(login|cadastro)\/?$/i.test(requestUrl.pathname);

  if (!token || !isTrustedApiRequest) {
    return next(req);
  }

  const requisicaoAutenticada = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(requisicaoAutenticada);
};