import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ServiceLogout {
  private readonly apiUrl = 'https://localhost:7094/api/auth/logout';

  constructor(private http: HttpClient) {}

  logout(): Observable<void> {
    return this.http.post<void>(this.apiUrl, {});
  }
}
