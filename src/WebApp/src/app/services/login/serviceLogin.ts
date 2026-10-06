import { HttpClient } from '@angular/common/http'; //ferramenta responsavel por fazer requisições http
import { Injectable } from '@angular/core'; // injecao de dependencia
import { Observable } from 'rxjs'; //é para funcoes async
import { loginRequest, LoginResponse } from './models/loginRequest';

@Injectable({
  providedIn: 'root', //cria uma instancia que sera usada pelo resto da aplicacao
})
export class serviceLogin {
  private readonly apiUrl = 'https://localhost:7094/api/auth/login';

  constructor(private http: HttpClient) {} //aqui está pedindo para o Angular fornecer uma instância de HttpClient

  //funcao enviarMensagem recebe uma variavel de dados do tipo loginRequest (record) : ela é uma função Observable<void> = async
  enviarMensagem(dados: loginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('https://localhost:7094/api/auth/login', dados); //o obj http abre a apiUrl no method post e envia os dados da interface
  }
}
