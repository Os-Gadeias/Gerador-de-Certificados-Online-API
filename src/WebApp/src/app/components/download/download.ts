import { Component, signal } from '@angular/core';

type CourseId = 'angular' | 'dotnet' | 'database';

@Component({
  imports: [],
  selector: 'app-download',
  templateUrl: './download.html',
})
export class Download {
  // Armazena o curso selecionado
  protected readonly selectedCourse = signal<CourseId | null>(null);

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
