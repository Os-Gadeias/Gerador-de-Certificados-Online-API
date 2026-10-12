import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  if (!token || !req.url.startsWith('https://localhost:7094/api/')) {
    return next(req);
  }

  const requisicaoAutenticada = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(requisicaoAutenticada);
};