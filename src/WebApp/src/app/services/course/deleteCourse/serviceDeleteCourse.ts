import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ServiceDeleteCourse {
  constructor(private http: HttpClient) {}

  private readonly apiUrl = 'https://localhost:7094/api/curso/';

  deleteCourse(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}${id}`); //concatenacao estranha de string
  }
}
