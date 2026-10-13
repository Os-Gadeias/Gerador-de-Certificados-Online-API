import { Component, inject, Signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { toSignal } from '@angular/core/rxjs-interop';
import { ListCourseResponse } from '../../services/course/models/Course';
import { map } from 'rxjs';
import { ProvideUser } from '../../auth/provideUser';
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

  protected readonly courses = toSignal(
    this.listCourses
      .enviarMensagem()
      .pipe(map((course: ListCourseResponse[]) => course.map(paraViewModel))),
    {
      initialValue: [] as ListCourseResponse[],
    },
  );
}
