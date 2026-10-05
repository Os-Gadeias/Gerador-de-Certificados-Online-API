import { Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { RouterLink } from '@angular/router';

import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ServiceCreateUser } from '../../services/createUser/serviceCreateUser';
import { HttpErrorResponse } from '@angular/common/http';
import { ApiError } from '../../services/apiError/apiError';
@Component({
  selector: 'app-createAccount',
  standalone: true,
  imports: [Footer, RouterLink, ReactiveFormsModule],
  templateUrl: './createAccount.html',
})
export class createAccount {
  public mensagemErro!: string;

  public formulario!: FormGroup;
  constructor(
    private createUser: FormBuilder,
    private serviceCreateUser: ServiceCreateUser,
  ) {
    this.formulario = createUser.group({
      email: ['', Validators.required],
      senha: ['', Validators.required],
    });
  }
  sendCreateUser(): void {
    if (this.formulario.invalid) {
      return;
    }
    const dados = this.formulario.value;

    this.serviceCreateUser.enviarMensagem(dados).subscribe({
      next: (resposta) => {
        console.log('Usuário criado!');
        console.log(resposta.usuarioId);

        this.mensagemErro = '';
      },

      error: (erro: HttpErrorResponse) => {
        const apiError = erro.error as ApiError;

        const mensagem = Object.values(apiError.errors)[0][0];

        this.mensagemErro = mensagem;
      },
    });
  }
}
