import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface Icons {
  titulo: string;
  simbol: string;
}

@Component({
  imports: [RouterLink],
  selector: 'app-home',
  templateUrl: './home.html',
})
export class Home {
  public readonly Itens: Icons[] = [
    { titulo: 'Generete Certificades', simbol: 'bi bi-bookmark-fill' },
    { titulo: 'Download in PDF', simbol: 'bi bi-filetype-pdf' },
    { titulo: 'Quick to Use!', simbol: 'bi bi-speedometer2' },
  ];
}
