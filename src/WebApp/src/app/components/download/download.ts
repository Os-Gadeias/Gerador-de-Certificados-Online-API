import { Component, inject, signal } from '@angular/core';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

type CourseId = 'angular' | 'dotnet' | 'database';

interface ListCourseViewModel {
  id: string;
  nome: string;
  descricao: string | null;
}

function toCourseViewModel(respose: ListCourseResponse[]): ListCourseViewModel[] {
  return respose.map((r) => ({
    id: r.id,
    nome: r.nome,
    descricao: r.descricao,
  }));
}

@Component({
  imports: [],
  selector: 'app-download',
  templateUrl: './download.html',
})
export class Download {
  protected listCourses = inject(ServiceListCourses);

  constructor() {}

  // Armazena o curso selecionado
  protected readonly selectedCourse = signal<CourseId | null>(null);

  //listagem de cursos
  protected courses = toSignal(
    this.listCourses.enviarMensagem().pipe(map((c) => toCourseViewModel(c))),
  );

  // Atualiza o curso selecionado na interface
  protected selectCourse(courseId: CourseId): void {
    this.selectedCourse.set(courseId);
  }

  // TODO: Integrar com a API de download de certificados
  protected downloadCertificates(): void {
    const courseId = this.selectedCourse();

    if (courseId !== 'angular') {
      return;
    }

    // TODO: Substituir pelo ID real do curso.
    // TODO: Chamar o service de download do ZIP.
    // TODO: Criar um Blob e iniciar o download no navegador.
    console.log('Download requested for course:', courseId);
  }
}
