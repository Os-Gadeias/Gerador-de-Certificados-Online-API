import { HttpClient } from '@angular/common/http';
import { CreateCourseRequest, CreateCourseResponse } from '../models/Course';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class serviceCreateCourse {
  private readonly apiUrl = 'https://localhost:7094/api/curso';

  constructor(private http: HttpClient) {}

  enviarMensagem(req: CreateCourseRequest): Observable<CreateCourseResponse> {
    return this.http.post<CreateCourseResponse>(this.apiUrl, req);
  }
}
