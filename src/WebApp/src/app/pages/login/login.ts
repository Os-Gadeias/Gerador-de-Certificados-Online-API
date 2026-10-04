import { Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login', //define o nome da tag html
  standalone: true, //com esse cara verdadeiro pode-se usar os imports abaixo
  imports: [Footer, RouterLink],
  templateUrl: './login.html', //o html relacionado a esse typescript está nessa propria pasta /login/login.html
})
export class Login {}
