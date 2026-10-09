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
      this.mensagemErro = 'The email and password fields are required!';
      return;
    }

    const dadosLogin = this.formularioLogin.value; //pega

    this.loginService.enviarMensagem(dadosLogin).subscribe({
      //o subscribe diz que quando a api responder ele vai fazer algo
      next: (resposta) => {
        //o next é quando o resultado da api é bem sucedido, ele tem um atributo reposta que é tipado pelo retorno da funcao enviar msg
        localStorage.setItem('token', resposta.accessToken);

        this.router.navigate(['/']);
        return;
      },

      error: (erro) => {
        const apiError = erro.error as ApiError; //pega a lista de erros e diz que ela é do tipo ApiError interface

        // pega o obj dos apiErros e transforma em um Array depois pega o array
        // [0] [0] pega o item erro e o primeiro item do array do erro

        const mensagem = Object.values(apiError.errors)[0][0];

        this.mensagemErro = mensagem;
        this.changeDetectionStrategy.markForCheck(); //tem que colocar esse cara para
        return;
      },
    });
  }
}
