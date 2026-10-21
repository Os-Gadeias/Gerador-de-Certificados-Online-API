import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CertificadeResponse, ListCertificadesResponse } from './models';

@Injectable({
  providedIn: 'root',
})
export class ServiceListStudents {
  private readonly urlApi = 'https://localhost:7094/api/certificado/';
  constructor(private http: HttpClient) {}
  enviarMensagem(id: string): Observable<ListCertificadesResponse> {
    return this.http.get<ListCertificadesResponse>(this.urlApi + id + '/certificados');
  }
}
