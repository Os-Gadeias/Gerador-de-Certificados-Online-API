import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormGroup, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { serviceCreateCourse } from '../../services/course/createCourse/serviceCreateCourse';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { HttpErrorResponse } from '@angular/common/http';

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
      error: (erro: HttpErrorResponse) => {
        const mensagem = extrairMensagemErro(erro);
        console.log(mensagem);
        this.mensagemErro = mensagem;
        this.changeDetector.markForCheck();
      },
    });
  }
}
