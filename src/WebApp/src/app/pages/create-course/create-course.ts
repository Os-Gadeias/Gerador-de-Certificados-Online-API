import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { serviceCreateCourse } from '../../services/course/createCourse/serviceCreateCourse';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { HttpErrorResponse } from '@angular/common/http';
import { ProvideUser } from '../../auth/provideUser';
import { form, FormField, minLength, required, validate } from '@angular/forms/signals';

interface CreateCourseModel {
  nome: string;
  descricao: string;
  cargaHoraria: number;
  dataConclusao: string;
}

@Component({
  imports: [RouterLink, FormField],
  selector: 'app-create-course',
  templateUrl: './create-course.html',
})
export class CreateCourse {
  mensagemErro = '';

  protected modelForm = signal<CreateCourseModel>({
    nome: '',
    descricao: '',
    cargaHoraria: 0,
    dataConclusao: '',
  });

  protected formCreateCourse = form(this.modelForm, (model) => {
    required(model.nome, { message: 'Enter the course name.' });
    required(model.cargaHoraria, { message: 'Enter the course hours.' });
    required(model.dataConclusao, { message: 'Enter the completion date.' });

    minLength(model.nome, 3, { message: 'Course name must have at least 3 characters' });
    minLength(model.descricao, 3, { message: 'Description name must have at least 3 characters' });

    validate(model.cargaHoraria, (c) => {
      if (c.value() <= 1) {
        return {
          kind: 'cargaHoraria',
          message: 'Hour must be greater then 1.',
        };
      }
      return null;
    });
  });

  constructor(
    private readonly servicoCadastroCurso: serviceCreateCourse,
    private router: Router,
    private provideUser: ProvideUser,
  ) {
    if (!provideUser.isLoggedIn()) {
      router.navigate(['/login']);
    }
  }

  protected sendCreateCourse(event: SubmitEvent) {
    event.preventDefault();

    if (!this.formCreateCourse().valid) return;

    const dados = this.modelForm();

    this.servicoCadastroCurso.enviarMensagem(dados).subscribe({
      next: (resposta) => {
        console.log(resposta.id);
        this.router.navigate(['/main'], {
          fragment: 'courses', //faz com que ele va pra section que queremos
        });
      },
      error: (erro: HttpErrorResponse) => {
        const mensagem = extrairMensagemErro(erro);
        console.log(mensagem);
        this.mensagemErro = mensagem;
      },
    });
  }
}
