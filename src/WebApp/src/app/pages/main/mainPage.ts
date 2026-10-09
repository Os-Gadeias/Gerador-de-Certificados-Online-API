import { Component } from '@angular/core';
import { Home } from '../../components/home/home';
import { Courses } from '../../components/courses/courses';
import { Navbar } from '../../components/navbar/navbar';
import { Footer } from '../../components/footer/footer';
import { Card } from '../../components/card/card';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [Navbar, Home, Footer, Card, Courses],
  templateUrl: './mainPage.html',
})
export class MainPage {}
