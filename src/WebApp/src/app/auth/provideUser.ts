import { computed, inject, Injectable, signal } from '@angular/core';
import { ServiceStorage } from './serivceStorage';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class ProvideUser {
  private readonly storage = inject(ServiceStorage);
  private readonly router = inject(Router);

  private readonly _username = signal<string | null>(this.storage.get('username'));

  readonly username = this._username.asReadonly();

  readonly isLoggedIn = computed(
    () => this._username() !== null && this.storage.get('token') !== null,
  );

  login(token: string, username: string): void {
    this.storage.save('token', token);
    this.storage.save('username', username);

    this._username.set(username);
  }

  logout(): void {
    this.storage.clear();
    this._username.set(null);

    this.router.navigate(['/login']);
  }
}
