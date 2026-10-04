import { Routes } from '@angular/router';
import { MainPage } from './pages/main/mainPage';
import { Login } from './pages/login/login';
import { createAccount } from './pages/createAccount/createAccount';

export const routes: Routes = [
  { path: '', component: MainPage },
  { path: 'login', component: Login },
  { path: 'createAccount', component: createAccount },
];
