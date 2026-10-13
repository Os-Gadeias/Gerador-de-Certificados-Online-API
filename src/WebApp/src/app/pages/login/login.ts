import { ChangeDetectorRef, Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { serviceLogin } from '../../services/login/serviceLogin';
import { HttpErrorResponse } from '@angular/common/http';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { ProvideUser } from '../../auth/provideUser';

@Component({
  selector: 'app-login', //define o nome da tag html
  standalone: true, //com esse cara verdadeiro pode-se usar os imports abaixo
  imports: [Footer, RouterLink, ReactiveFormsModule],
  templateUrl: './login.html', //o html relacionado a esse typescript está nessa propria pasta /login/login.html,
})
export class Login {
  public formularioLogin!: FormGroup;
  mensagemErro!: string;

  constructor(
    private login: FormBuilder,
    private loginService: serviceLogin,
    private router: Router,
    private changeDetector: ChangeDetectorRef,
    private provideUser: ProvideUser,
  ) {
    this.formularioLogin = this.login.group({
      email: ['', Validators.required],
      senha: ['', Validators.required],
    });
  }
  sendUserLogin(): void {
    if (this.formularioLogin.invalid) {
      this.mensagemErro = 'The email and password fields are required!';
      return;
    }

    const dadosLogin = this.formularioLogin.value; //pega

    this.loginService.enviarMensagem(dadosLogin).subscribe({
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
          this.changeDetector.markForCheck();
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
        this.changeDetector.markForCheck();
        return;
      },
    });
  }
}
