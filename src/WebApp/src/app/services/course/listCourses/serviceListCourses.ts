import { HttpClient } from '@angular/common/http';
import { ListCourseResponse } from '../models/Course';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServiceListCourses {
  private readonly apiUrl = 'https://localhost:7094/api/curso';

  constructor(private http: HttpClient) {}

  enviarMensagem(): Observable<ListCourseResponse[]> {
    return this.http.get<ListCourseResponse[]>(this.apiUrl, {});
  }
}
