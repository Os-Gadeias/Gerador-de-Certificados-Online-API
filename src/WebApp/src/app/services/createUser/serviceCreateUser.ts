import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createUserCommand, CreateUserResponse } from './models/createUser';

@Injectable({ providedIn: 'root' })
export class ServiceCreateUser {
  private readonly apiUrl = 'https://localhost:7094/api/auth/cadastro';

  constructor(private http: HttpClient) {}

  enviarMensagem(dados: createUserCommand): Observable<CreateUserResponse> {
    return this.http.post<CreateUserResponse>('https://localhost:7094/api/auth/cadastro', dados);
  }
}
