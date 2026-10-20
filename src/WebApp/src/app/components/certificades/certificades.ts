import { Component, inject, signal } from '@angular/core';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { ServiceListStudents } from '../../services/certificade/serviceListStudents';
import { HttpErrorResponse } from '@angular/common/http';
import { ListCertificadesResponse } from '../../services/certificade/models';
import { form, FormField, required } from '@angular/forms/signals';
import { ServiceEnrollStudents } from '../../services/certificade/serviceEnrollStudent';

interface EnrollStudentViewModel {
  cursoId: string;
  students: string;
}

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
  imports: [FormField],
  selector: 'app-certificades',
  templateUrl: './certificades.html',
})
export class Certificades {
  protected listCourse = inject(ServiceListCourses);
  protected listStudents = inject(ServiceListStudents);
  protected enrollStudent = inject(ServiceEnrollStudents);

  protected students = signal<StudentViewModel[]>([]);
  protected selectedCourseId = signal<string | null>(null);
  protected success = signal<boolean>(false);

  protected modelForm = signal<EnrollStudentViewModel>({
    cursoId: '',
    students: '',
  });

  protected formEnrolStudents = form(this.modelForm, (model) => {
    required(model.cursoId, { message: 'Select a Course.' });
    required(model.students, { message: 'Select enter a student name' });
  });

  protected courses = toSignal(
    this.listCourse.enviarMensagem().pipe(map((c: ListCourseResponse[]) => c.map(ToViewModel))),
  );

  onCourseChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    const courseId = select.value;

    this.selectedCourseId.set(courseId);
    this.students.set([]);

    this.loadStudents(courseId);
  }

  private loadStudents(courseId: string) {
    this.listStudents.enviarMensagem(courseId).subscribe({
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

  EnrolStudent(event: SubmitEvent) {
    event.preventDefault();
    const courseId = this.formEnrolStudents.cursoId().value();

    const students = this.formEnrolStudents.students().value();

    const studentsList = students
      .split(',')
      .map((name) => name.trim())
      .filter((name) => name.length > 2);

    if (!courseId || studentsList.length === 0) {
      return;
    }

    this.enrollStudent.enviarMensagem(courseId, studentsList).subscribe({
      next: () => {
        this.success.set(true);
        this.loadStudents(courseId);
      },
      error: (err: HttpErrorResponse) => {
        this.success.set(false);
        console.error('Error enrolling students:', err);
      },
    });
  }
}
