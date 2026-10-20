import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { toSignal } from '@angular/core/rxjs-interop';
import { ListCourseResponse } from '../../services/course/models/Course';
import { map } from 'rxjs';
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
  protected readonly courses = toSignal(
    this.listCourses
      .enviarMensagem()
      .pipe(map((course: ListCourseResponse[]) => course.map(paraViewModel))),
    {
      initialValue: [] as ListCourseResponse[],
    },
  );
  protected deleteCourse(id: string) {
    this.serviceDelete.deleteCourse(id).subscribe((success) => {
      next: () => {
        console.log(`sucess: ${success}`);
      };
      error: (erro: HttpErrorResponse) => {
        console.error(`Error:` + extrairMensagemErro(erro));
      };
    });
  }
}
