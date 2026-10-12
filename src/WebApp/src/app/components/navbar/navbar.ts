import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface NavBarComponent {
  titulo: string;
  url: string;
  icone: string;
}

@Component({
  imports: [],
  selector: 'app-navbar',
  templateUrl: './navbar.html',
})
export class Navbar {
  public readonly itens: NavBarComponent[] = [
    { titulo: 'Home', url: '#home', icone: 'bi-house-door-fill' },
    { titulo: 'Courses', url: '#courses', icone: 'bi-backpack' },
    { titulo: 'Certificate', url: '#Certificados', icone: 'bi-patch-check' },
  ];
}
