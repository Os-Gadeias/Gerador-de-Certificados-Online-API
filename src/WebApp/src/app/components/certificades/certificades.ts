import { Component, inject, signal } from '@angular/core';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { finalize, map, pipe } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { ServiceListStudents } from '../../services/certificade/serviceListStudents';
import { HttpErrorResponse } from '@angular/common/http';
import { CertificadeResponse, ListCertificadesResponse } from '../../services/certificade/models';

interface CoursesViewModel {
  id: string;
  name: string;
}
interface StudentViewModel {
  name: string;
  status: string;
}

function ToStudentModel(students: ListCertificadesResponse): StudentViewModel[] {
  return students.certificados.map((student) => ({
    name: student.nomeAluno,
    status: student.status,
  }));
}
function ToViewModel(course: ListCourseResponse): CoursesViewModel {
  return {
    id: course.id,
    name: course.nome,
  };
}

@Component({
  imports: [],
  selector: 'app-certificades',
  templateUrl: './certificades.html',
})
export class Certificades {
  protected listCourse = inject(ServiceListCourses);
  protected listStudents = inject(ServiceListStudents);

  protected students = signal<StudentViewModel[]>([]);
  protected selectedCourseId = signal<string | null>(null);
  protected loading = signal(false);

  protected courses = toSignal(
    this.listCourse.enviarMensagem().pipe(map((c: ListCourseResponse[]) => c.map(ToViewModel))),
  );

  onCourseChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    const courseId = select.value;

    this.selectedCourseId.set(courseId);
    this.students.set([]);
    this.loading.set(true);

    this.listStudents
      .enviarMensagem(courseId)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (students) => {
          if (this.selectedCourseId() === courseId) {
            this.students.set(ToStudentModel(students));
          }
        },
        error: (error: HttpErrorResponse) => {
          console.error('Error loading students:', error);
        },
      });
  }
}
