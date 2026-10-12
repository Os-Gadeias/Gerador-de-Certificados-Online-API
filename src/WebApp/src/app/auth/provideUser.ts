import { Injectable } from '@angular/core';
import { ServiceStorage } from './serivceStorage';

@Injectable({
  providedIn: 'root',
})
export class ProvideUser {
  constructor(private storage: ServiceStorage) {}

  userIsLogged(): boolean {
    return Boolean(this.storage.get('token'));
  }
}
