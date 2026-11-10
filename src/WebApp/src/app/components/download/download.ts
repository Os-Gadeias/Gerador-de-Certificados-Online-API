import { Component, computed, inject, signal } from '@angular/core';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { EMPTY, catchError, exhaustMap, map, of, switchMap, tap, timer } from 'rxjs';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { ServiceListCertificades } from '../../services/certificade/serviceListCertificades';
import { ListCertificadesResponse } from '../../services/certificade/models';
import { ServiceDownloadCertificades } from '../../services/certificade/serviceDownloadCertificades';
import { HttpErrorResponse } from '@angular/common/http';
import { ProvideUser } from '../../auth/provideUser';
import { RouterLink } from '@angular/router';

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
  imports: [RouterLink],
  selector: 'app-download',
  templateUrl: './download.html',
})
export class Download {
  private readonly refreshIntervalMs = 3000;
  protected provideUser = inject(ProvideUser);
  protected listCourses = inject(ServiceListCourses);
  protected listCertificades = inject(ServiceListCertificades);
  protected downloadCertificades = inject(ServiceDownloadCertificades);

  // signal para o ID do curso

  protected readonly selectedCourse = signal<string | null>(null);

  protected readonly countCourses = computed(() => this.courses().length);

  protected certificades = signal<CertificadeViewModel[] | null>(null);
  protected readonly statusError = signal(false);
  protected readonly loadingCertificades = signal(false);
  protected readonly coursesError = signal(false);

  protected readonly countCertificades = computed(() => this.certificades()?.length ?? 0);
  protected readonly certificatesReady = computed(
    () =>
      this.countCertificades() > 0 && this.certificades()!.every((c) => c.status === 'Concluído'),
  );
  protected readonly hasFailures = computed(
    () => this.certificades()?.some((c) => c.status === 'Falha') ?? false,
  );

  // Atualiza os cursos e suas quantidades enquanto o usuário estiver autenticado.
  protected courses = toSignal(
    toObservable(this.provideUser.isLoggedIn).pipe(
      switchMap((isLoggedIn) =>
        isLoggedIn
          ? timer(0, this.refreshIntervalMs).pipe(
              exhaustMap(() =>
                this.listCourses.enviarMensagem().pipe(
                  map((courses) => toCourseViewModel(courses)),
                  tap(() => this.coursesError.set(false)),
                  catchError(() => {
                    this.coursesError.set(true);
                    // Preserva a lista atual e tenta novamente na próxima consulta.
                    return EMPTY;
                  }),
                ),
              ),
            )
          : of([] as ListCourseViewModel[]),
      ),
    ),
    { initialValue: [] as ListCourseViewModel[] },
  );

  constructor() {
    toObservable(this.selectedCourse)
      .pipe(
        switchMap((courseId) =>
          courseId === null
            ? EMPTY
            : timer(0, this.refreshIntervalMs).pipe(
                exhaustMap(() => this.loadCertificades(courseId)),
              ),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((response) => {
        this.certificades.set(toCertificadeViewModel(response));
        this.statusError.set(false);
        this.loadingCertificades.set(false);
      });
  }

  // Trocar o curso cancela a consulta anterior e reinicia o acompanhamento da fila.
  protected selectCourse(courseId: string): void {
    if (this.selectedCourse() === courseId) return;
    this.certificades.set(null);
    this.statusError.set(false);
    this.loadingCertificades.set(true);
    this.selectedCourse.set(courseId);
  }
  protected downloadCertificates(courseId: string) {
    if (!this.certificatesReady() || this.statusError()) return;
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
    return this.listCertificades.enviarMensagem(courseId).pipe(
      catchError(() => {
        this.statusError.set(true);
        this.loadingCertificades.set(false);
        // Mantém os últimos status e tenta novamente na próxima consulta.
        return EMPTY;
      }),
    );
  }

  protected certificateStatus(status: string) {
    switch (status) {
      case 'NaoIniciado':
        return { label: 'Not started', color: 'text-secondary', icon: 'bi-clock' };
      case 'Pendente':
        return { label: 'Queued', color: 'text-warning', icon: 'bi-hourglass-split' };
      case 'GerandoCertificado':
        return { label: 'Generating certificate', color: 'text-warning', icon: 'bi-gear' };
      case 'GerandoZip':
        return { label: 'Generating ZIP', color: 'text-info', icon: 'bi-file-earmark-zip' };
      case 'Concluído':
        return { label: 'Completed', color: 'text-success', icon: 'bi-check-circle-fill' };
      case 'Falha':
        return {
          label: 'Generation failed',
          color: 'text-danger',
          icon: 'bi-exclamation-triangle',
        };
      default:
        return { label: status, color: 'text-secondary', icon: 'bi-question-circle' };
    }
  }
}
