import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ServiceDeleteCourse {
  private readonly deletedCourse = new Subject<string>();
  readonly courseDeleted$ = this.deletedCourse.asObservable();

  constructor(private http: HttpClient) {}

  private readonly apiUrl = 'https://localhost:7094/api/curso/';

  deleteCourse(id: string): Observable<boolean> {
    return this.http
      .delete<boolean>(`${this.apiUrl}${id}`)
      .pipe(tap(() => this.deletedCourse.next(id)));
  }
}
