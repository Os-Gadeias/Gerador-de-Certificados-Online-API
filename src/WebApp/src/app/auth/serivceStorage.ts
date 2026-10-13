import { Injectable } from '@angular/core';

type AuthStorageKey = 'token' | 'username';

@Injectable({
  providedIn: 'root',
})
export class ServiceStorage {
  constructor() {
    if (typeof window !== 'undefined') {
      window.localStorage.removeItem('token');
      window.localStorage.removeItem('username');
    }
  }

  save(chave: AuthStorageKey, valor: string): void {
    const valorNormalizado = valor.trim();
    if (!valorNormalizado || (chave === 'token' && valorNormalizado !== valor)) {
      throw new TypeError(`O valor de "${chave}" é inválido.`);
    }

    this.getStorage().setItem(chave, valorNormalizado);
  }

  get(chave: AuthStorageKey): string | null {
    const valor = this.getStorage().getItem(chave);
    if (chave === 'token' && valor !== null && (!valor.trim() || valor !== valor.trim())) {
      this.remove(chave);
      return null;
    }

    return valor;
  }

  remove(chave: AuthStorageKey): void {
    this.getStorage().removeItem(chave);
  }

  clear(): void {
    this.remove('token');
    this.remove('username');
  }

  private getStorage(): Storage {
    if (typeof window === 'undefined') {
      throw new Error('O armazenamento de sessão só está disponível no navegador.');
    }

    return window.sessionStorage;
  }
}
