import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from './components/navbar/navbar';
import { Home } from './components/home/home';
import { Courses } from './components/courses/courses';

@Component({
  imports: [Navbar, Home, Courses],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('WebApp');
}
