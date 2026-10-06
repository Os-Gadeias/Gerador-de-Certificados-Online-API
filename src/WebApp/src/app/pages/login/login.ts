import { ChangeDetectorRef, Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { serviceLogin } from '../../services/login/serviceLogin';
import { ApiError } from '../../services/apiError/apiError';

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
    private changeDetectionStrategy: ChangeDetectorRef,
  ) {
    this.formularioLogin = this.login.group({
      email: ['', Validators.required],
      senha: ['', Validators.required],
    });
  }
  sendUserLogin(): void {
    if (this.formularioLogin.invalid) {
      this.mensagemErro = 'Os campos Email e Senha são obrigatórios!';
      return;
    }

    const dadosLogin = this.formularioLogin.value;

    this.loginService.enviarMensagem(dadosLogin).subscribe({
      next: (resposta) => {
        localStorage.setItem('token', resposta.token);

        this.router.navigate(['/']);
        return;
      },

      error: (erro) => {
        const apiError = erro.error as ApiError;

        const mensagem = Object.values(apiError.errors)[0][0];

        this.mensagemErro = mensagem;
        this.changeDetectionStrategy.markForCheck();
        return;
      },
    });
  }
}
