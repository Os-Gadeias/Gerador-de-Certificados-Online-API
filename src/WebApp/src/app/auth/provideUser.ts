import { computed, inject, Injectable, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { ServiceStorage } from './serivceStorage';
import { Router } from '@angular/router';
import { ServiceLogout } from '../services/login/serviceLogout';

@Injectable({
  providedIn: 'root',
})
export class ProvideUser {
  private readonly storage = inject(ServiceStorage);
  private readonly router = inject(Router);
  private readonly logoutService = inject(ServiceLogout);

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
    this.logoutService
      .logout()
      .pipe(finalize(() => this.finalizarLogoutLocal()))
      .subscribe({
        error: (erro: unknown) => {
          console.error('Falha ao notificar a API sobre o logout.', erro);
        },
      });
  }

  private finalizarLogoutLocal(): void {
    this.storage.clear();
    this._username.set(null);
    void this.router.navigate(['/login']);
  }
}
