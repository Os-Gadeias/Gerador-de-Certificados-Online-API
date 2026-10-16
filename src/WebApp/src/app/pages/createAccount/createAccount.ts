import { Component, signal } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { Router, RouterLink } from '@angular/router';
import { ServiceCreateUser } from '../../services/createUser/serviceCreateUser';
import { HttpErrorResponse } from '@angular/common/http';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { ProvideUser } from '../../auth/provideUser';
import {
  email,
  form,
  FormField,
  minLength,
  pattern,
  required,
  validate,
} from '@angular/forms/signals';

interface CreateAccountForm {
  email: string;
  senha: string;
  senhaConfirmada: string;
}

@Component({
  selector: 'app-createAccount',
  standalone: true,
  imports: [Footer, RouterLink, FormField],
  templateUrl: './createAccount.html',
})
export class createAccount {
  mensagemErro!: string;

  protected readonly formModel = signal<CreateAccountForm>({
    email: '',
    senha: '',
    senhaConfirmada: '',
  });
  protected readonly formCreateAccount = form(this.formModel, (model) => {
    required(model.email, { message: 'Enter your email.' });
    required(model.senha, { message: 'Enter your password.' });
    required(model.senhaConfirmada, { message: 'Enter your password.' });
    email(model.email, { message: 'Enter a valid email.' });
    minLength(model.senha, 8, { message: 'The password must have at least 8 characters' });
    pattern(model.senha, /(?=.*\d)(?=.*[^a-zA-Z0-9]).+/, {
      message: 'Include at least one number and special characters',
    });
    validate(model.senhaConfirmada, ({ value, valueOf }) => {
      const senha = valueOf(model.senha);
      const senhaConfirmada = value();

      if (senha !== senhaConfirmada) {
        return {
          kind: 'senhaConfirmada',
          message: 'The passwords dont match!',
        };
      }
      return null;
    });
  });

  constructor(
    private serviceCreateUser: ServiceCreateUser,
    private router: Router,
    private provideUser: ProvideUser,
  ) {
    if (provideUser.isLoggedIn()) router.navigate(['/']);
  }
  sendCreateUser(event: SubmitEvent): void {
    event.preventDefault();

    const data = this.formModel();

    this.serviceCreateUser.enviarMensagem(data).subscribe({
      next: (resposta) => {
        if (
          !resposta ||
          typeof resposta.accessToken !== 'string' ||
          !resposta.accessToken.trim() ||
          typeof resposta.nomeUsuario !== 'string' ||
          !resposta.nomeUsuario.trim()
        ) {
          this.mensagemErro = 'A API retornou dados de autenticação inválidos.';
          return;
        }

        this.provideUser.login(resposta.accessToken, resposta.nomeUsuario);

        this.router.navigate(['/']);
      },

      error: (erro: HttpErrorResponse) => {
        const mensagem = extrairMensagemErro(erro);
        console.log(mensagem);
        this.mensagemErro = mensagem;
      },
    });
  }
}
