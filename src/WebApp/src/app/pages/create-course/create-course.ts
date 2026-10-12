import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormGroup, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { serviceCreateCourse } from '../../services/course/createCourse/serviceCreateCourse';
import { ApiError } from '../../services/api/apiError/apiError';

@Component({
  imports: [RouterLink, ReactiveFormsModule],
  selector: 'app-create-course',
  templateUrl: './create-course.html',
})
export class CreateCourse {
  public formulario!: FormGroup;

  mensagemErro = '';

  constructor(
    private readonly servicoCadastroCurso: serviceCreateCourse,
    private createGroup: FormBuilder,
    private changeDetector: ChangeDetectorRef,
    private router: Router,
  ) {
    this.formulario = createGroup.group({
      nome: ['', Validators.required],
      cargaHoraria: ['', Validators.required],
      dataConclusao: ['', Validators.required],
    });
  }

  protected sendCreateCourse() {
    if (this.formulario.invalid) {
      this.mensagemErro = 'All fields are required!';
      return;
    }
    const dados = this.formulario.value;

    this.servicoCadastroCurso.enviarMensagem(dados).subscribe({
      next: (resposta) => {
        console.log(resposta.id);
        this.router.navigate(['/']);
      },
      error: (erro) => {
        const apiError = erro.error as ApiError;

        const mensagem = Object.values(apiError.errors)[0][0];

        this.mensagemErro = mensagem;
        this.changeDetector.markForCheck();
      },
    });
  }
}
