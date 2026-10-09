import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'main', pathMatch: 'full' },
  {
    path: 'main',
    loadComponent: () => import('./pages/main/mainPage').then((component) => component.MainPage),
  },
  { path: 'login', redirectTo: 'loginPage' },
  {
    path: 'loginPage',
    loadComponent: () => import('./pages/login/login').then((component) => component.Login),
  },
  { path: 'createAccount', redirectTo: 'createAccountPage' },
  {
    path: 'createAccountPage',
    loadComponent: () => import('./pages/createAccount/createAccount').then((c) => c.createAccount),
  },
];
