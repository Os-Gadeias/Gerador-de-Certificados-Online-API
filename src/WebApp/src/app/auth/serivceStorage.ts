import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceStorage {
  save(chave: string, valor: string): void {
    localStorage.setItem(chave, valor);
  }

  get(chave: string): string | null {
    return localStorage.getItem(chave);
  }

  remove(chave: string): void {
    localStorage.removeItem(chave);
  }

  clear(): void {
    localStorage.clear();
  }
}
