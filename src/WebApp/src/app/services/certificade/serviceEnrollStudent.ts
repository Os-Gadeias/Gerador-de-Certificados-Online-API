import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnrollStudentsResponse } from './models';

@Injectable({
  providedIn: 'root',
})
export class ServiceEnrollStudents {
  private readonly urlApi = 'https://localhost:7094/api/certificado/';

  constructor(private http: HttpClient) {}

  enviarMensagem(id: string, students: string[]): Observable<EnrollStudentsResponse> {
    return this.http.post<EnrollStudentsResponse>(`${this.urlApi}${id}`, {
      nomesAlunos: students,
    });
  }
}
