import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref } from '@angular/router';
import { Navbar } from './components/navbar/navbar';
import { Home } from './components/home/home';
import { Courses } from './components/courses/courses';
import { Footer } from './components/footer/footer';

@Component({
  imports: [Navbar, Home, Courses, RouterOutlet, RouterLinkWithHref, Footer],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('WebApp');
}
