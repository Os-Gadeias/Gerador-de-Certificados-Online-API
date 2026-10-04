import { Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-createAccount',
  standalone: true,
  imports: [Footer, RouterLink],
  templateUrl: './createAccount.html',
})
export class createAccount {}
