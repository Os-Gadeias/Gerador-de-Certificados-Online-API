import { Component, inject, signal } from '@angular/core';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

interface CoursesViewModel {
  id: string;
  name: string;
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
  protected ListCourse = inject(ServiceListCourses);

  protected courses = toSignal(
    this.ListCourse.enviarMensagem().pipe(map((c: ListCourseResponse[]) => c.map(ToViewModel))),
  );
}
