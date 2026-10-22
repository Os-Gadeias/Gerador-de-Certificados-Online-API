import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ListCertificadesResponse } from './models';

@Injectable({
  providedIn: 'root',
})
export class ServiceListCertificades {
  private apiUrl = 'https://localhost:7094/api/certificado/';

  constructor(private http: HttpClient) {}

  enviarMensagem(cursoId: string): Observable<ListCertificadesResponse> {
    return this.http.get<ListCertificadesResponse>(`${this.apiUrl}${cursoId}/certificados`);
  }
}
