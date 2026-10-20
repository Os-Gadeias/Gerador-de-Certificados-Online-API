import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ServiceEnrollStudents {
  private urlApi = 'https://localhost:7094/api/certificado/';

  constructor(private http: HttpClient) {}

  enviarMensagem(id: string, students: string[]): Observable<boolean> {
    return this.http.post<boolean>(`${this.urlApi}${id}`, students);
  }
}
