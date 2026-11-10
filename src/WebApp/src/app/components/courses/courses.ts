import { Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ListCourseResponse } from '../../services/course/models/Course';
import { finalize } from 'rxjs';
import { ProvideUser } from '../../auth/provideUser';
import { ServiceDeleteCourse } from '../../services/course/deleteCourse/serviceDeleteCourse';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { HttpErrorResponse } from '@angular/common/http';

interface ListCourseViewModel {
  id: string;
  nome: string;
  descricao: string | null;
  cargaHoraria: number;
  dataConclusao: string;
}

function paraViewModel(dto: ListCourseResponse): ListCourseViewModel {
  return {
    id: dto.id,
    nome: dto.nome.toLocaleLowerCase(),
    descricao: dto.descricao,
    cargaHoraria: dto.cargaHoraria,
    dataConclusao: dto.dataConclusao.split('T')[0],
  };
}

@Component({
  imports: [RouterLink],
  selector: 'app-courses',
  templateUrl: './courses.html',
})
export class Courses {
  readonly provideUser = inject(ProvideUser);
  readonly listCourses = inject(ServiceListCourses);
  readonly serviceDelete = inject(ServiceDeleteCourse);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly courses = signal<ListCourseViewModel[]>([]);
  protected readonly deletingCourses = signal<Set<string>>(new Set());
  protected readonly errorMessage = signal<string | null>(null);

  constructor() {
    this.listCourses
      .enviarMensagem()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (courses) => this.courses.set(courses.map(paraViewModel)),
        error: (erro: HttpErrorResponse) => this.errorMessage.set(extrairMensagemErro(erro)),
      });
  }

  protected deleteCourse(id: string): void {
    if (this.deletingCourses().has(id)) return;

    this.errorMessage.set(null);
    this.deletingCourses.update((ids) => new Set(ids).add(id));

    this.serviceDelete
      .deleteCourse(id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.deletingCourses.update((ids) => {
            const updatedIds = new Set(ids);
            updatedIds.delete(id);
            return updatedIds;
          });
        }),
      )
      .subscribe({
        next: () => {
          this.courses.update((courses) => courses.filter((course) => course.id !== id));
        },
        error: (erro: HttpErrorResponse) => {
          this.errorMessage.set(extrairMensagemErro(erro));
        },
      });
  }
}
