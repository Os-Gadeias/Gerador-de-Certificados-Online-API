import { Component } from '@angular/core';
import { Home } from '../../components/home/home';
import { Courses } from '../../components/courses/courses';
import { Navbar } from '../../components/navbar/navbar';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [Navbar, Home, Courses],
  templateUrl: './mainPage.html',
})

export class MainPage {}
