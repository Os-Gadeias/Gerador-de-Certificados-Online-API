import { Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { serviceLogin } from '../../services/login/serviceLogin';

@Component({
  selector: 'app-login', //define o nome da tag html
  standalone: true, //com esse cara verdadeiro pode-se usar os imports abaixo
  imports: [Footer, RouterLink, ReactiveFormsModule],
  templateUrl: './login.html', //o html relacionado a esse typescript está nessa propria pasta /login/login.html
})
export class Login {
  public formularioLogin!: FormGroup;

  constructor(
    private login: FormBuilder,
    private loginService: serviceLogin,
  ) {
    this.formularioLogin = this.login.group({
      email: ['', Validators.required],
      senha: ['', Validators.required],
    });
  }
  sendUserLogin(): void {
    if (this.formularioLogin.invalid) return;

    const dadosLogin = this.formularioLogin.value;

    this.loginService.enviarMensagem(dadosLogin).subscribe({
      next: () => {
        console.log('send to the API');
        return;
      },

      error: (erro) => {
        console.error("error couldn't access the api!");
        return;
      },
    });
  }
}
