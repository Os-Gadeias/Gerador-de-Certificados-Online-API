import { Routes } from '@angular/router';
import { MainPage } from './pages/main/mainPage';
import { Login } from './pages/login/login';

export const routes: Routes = [
  { path: '', component: MainPage },
  { path: 'login', component: Login },
];
