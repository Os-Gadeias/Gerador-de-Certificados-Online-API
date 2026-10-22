import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProvideUser } from '../../auth/provideUser';

interface NavBarComponent {
  titulo: string;
  url: string;
  icone: string;
}

@Component({
  imports: [RouterLink],
  selector: 'app-navbar',
  templateUrl: './navbar.html',
})
export class Navbar {
  public readonly itens: NavBarComponent[] = [
    { titulo: 'Home', url: '#home', icone: 'bi-house-door-fill' },
    { titulo: 'Courses', url: '#courses', icone: 'bi-backpack' },
    { titulo: 'Certificate', url: '#certificade', icone: 'bi-patch-check' },
    { titulo: 'Downloads', url: '#downloads', icone: 'bi-download' },
  ];
  constructor(protected provideUser: ProvideUser) {}
}
