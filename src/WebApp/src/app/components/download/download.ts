import { Component, computed, inject, signal } from '@angular/core';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { ServiceListCertificades } from '../../services/certificade/serviceListCertificades';
import { ListCertificadesResponse } from '../../services/certificade/models';
import { ServiceDownloadCertificades } from '../../services/certificade/serviceDownloadCertificades';
import { HttpErrorResponse } from '@angular/common/http';

interface ListCourseViewModel {
  id: string;
  nome: string;
  descricao: string | null;
  studentsCertificades: number | null;
}

interface CertificadeViewModel {
  nomeAluno: string;
  status: string;
}

function toCertificadeViewModel(response: ListCertificadesResponse): CertificadeViewModel[] {
  return response.certificados.map((c) => ({
    nomeAluno: c.nomeAluno,
    status: c.status,
  }));
}

function toCourseViewModel(respose: ListCourseResponse[]): ListCourseViewModel[] {
  return respose.map((r) => ({
    id: r.id,
    nome: r.nome,
    descricao: r.descricao,
    studentsCertificades: r.qntdCertificados,
  }));
}

@Component({
  imports: [],
  selector: 'app-download',
  templateUrl: './download.html',
})
export class Download {
  protected listCourses = inject(ServiceListCourses);
  protected listCertificades = inject(ServiceListCertificades);
  protected downloadCertificades = inject(ServiceDownloadCertificades);
  constructor() {}

  // signal para o ID do curso

  protected readonly selectedCourse = signal<string | null>(null);

  protected readonly countCourses = computed(() => this.courses().length);

  protected certificades = signal<CertificadeViewModel[] | null>(null);

  protected readonly countCertificades = computed(() => this.certificades()?.length ?? 0);

  //listagem de cursos
  protected courses = toSignal(
    this.listCourses.enviarMensagem().pipe(map((c) => toCourseViewModel(c))),
    { initialValue: [] as ListCourseViewModel[] },
  );

  // Atualiza o curso selecionado na interface
  protected selectCourse(courseId: string): void {
    this.selectedCourse.set(courseId);

    this.loadCertificades(courseId);
  }
  protected downloadCertificates(courseId: string) {
    this.downloadCertificades.enviarMensagem(courseId).subscribe({
      next: (blob: Blob) => {
        const url = URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = url;
        link.download = `certificates-${courseId}.zip`;

        document.body.appendChild(link);
        link.click();

        document.body.removeChild(link);
        URL.revokeObjectURL(url);
      },

      error: (err: HttpErrorResponse) => {
        console.error('Error downloading certificates:', err);
      },
    });
  }

  private loadCertificades(courseId: string) {
    this.listCertificades.enviarMensagem(courseId).subscribe({
      next: (response) => {
        this.certificades.set(toCertificadeViewModel(response));
      },
    });
  }
}

// TODO: Integrar com a API de download de certificados
