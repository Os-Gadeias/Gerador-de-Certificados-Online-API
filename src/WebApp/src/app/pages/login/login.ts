import { Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';

@Component({
  selector: 'app-login', //define o nome da tag html
  standalone: true, //com esse cara verdadeiro pode-se usar os imports abaixo
  imports: [Footer],
  templateUrl: './login.html', //o html relacionado a esse typescript está nessa propria pasta /login/login.html
})
export class Login {}
