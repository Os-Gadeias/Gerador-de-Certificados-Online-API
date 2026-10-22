import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceDownloadCertificades {
  private urlApi = 'https://localhost:7094/api/certificado/';

  constructor(private http: HttpClient) {}

  enviarMensagem(courseId: string) {
    return this.http.get(`${this.urlApi}${courseId}/download`, {
      responseType: 'blob',
    });
  }
}
