import { Component, signal } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { Router, RouterLink } from '@angular/router';
import { serviceLogin } from '../../services/login/serviceLogin';
import { HttpErrorResponse } from '@angular/common/http';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { ProvideUser } from '../../auth/provideUser';
import { form, FormField, required } from '@angular/forms/signals';

interface LoginFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  selector: 'app-login', //define o nome da tag html
  standalone: true, //com esse cara verdadeiro pode-se usar os imports abaixo
  imports: [Footer, RouterLink, FormField],
  templateUrl: './login.html', //o html relacionado a esse typescript está nessa propria pasta /login/login.html,
})
export class Login {
  protected mensagemErro = '';
  protected readonly modelForm = signal<LoginFormModel>({ email: '', senha: '' });
  protected readonly formLogin = form(this.modelForm, (model) => {
    required(model.email, { message: 'Insert your Email!' });
    required(model.senha, { message: 'Insert your Password!' });
  });
  constructor(
    private loginService: serviceLogin,
    private router: Router,
    private provideUser: ProvideUser,
  ) {}
  sendUserLogin(event: SubmitEvent): void {
    event.preventDefault();

    const valorFormulario = this.modelForm;

    this.loginService.enviarMensagem(valorFormulario()).subscribe({
      //o subscribe diz que quando a api responder ele vai fazer algo
      next: (resposta) => {
        //o next é quando o resultado da api é bem sucedido, ele tem um atributo reposta que é tipado pelo retorno da funcao enviar msg
        if (
          !resposta ||
          typeof resposta.accessToken !== 'string' ||
          !resposta.accessToken.trim() ||
          typeof resposta.username !== 'string' ||
          !resposta.username.trim()
        ) {
          this.mensagemErro = 'A API retornou dados de autenticação inválidos.';
          return;
        }

        this.provideUser.login(resposta.accessToken, resposta.username);
        this.router.navigate(['/']);
        return;
      },

      error: (erro: HttpErrorResponse) => {
        const mensagem = extrairMensagemErro(erro);
        console.log(mensagem);
        this.mensagemErro = mensagem;
        return;
      },
    });
  }
}
