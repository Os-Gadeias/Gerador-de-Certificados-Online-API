import { Component, Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProvideUser {
  constructor() {}
  userIsLogged(): boolean {
    return localStorage.getItem('token') !== null;
  }
}
