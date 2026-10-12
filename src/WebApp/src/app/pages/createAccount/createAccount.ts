import { ChangeDetectorRef, Component } from '@angular/core';
import { Footer } from '../../components/footer/footer';
import { Router, RouterLink } from '@angular/router';

import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ServiceCreateUser } from '../../services/createUser/serviceCreateUser';
import { HttpErrorResponse } from '@angular/common/http';
import { extrairMensagemErro } from '../../services/api/apiError/apiError';
import { ServiceStorage } from '../../auth/serivceStorage';

@Component({
  selector: 'app-createAccount',
  standalone: true,
  imports: [Footer, RouterLink, ReactiveFormsModule],
  templateUrl: './createAccount.html',
})
export class createAccount {
  mensagemErro!: string;

  public formulario!: FormGroup;
  constructor(
    private createUser: FormBuilder,
    private serviceCreateUser: ServiceCreateUser,
    private changeDetector: ChangeDetectorRef,
    private router: Router,
    private storage: ServiceStorage,
  ) {
    this.formulario = createUser.group({
      email: ['', Validators.required],
      senha: ['', Validators.required],
    });
  }
  sendCreateUser(): void {
    if (this.formulario.invalid) {
      this.mensagemErro = 'The email and password fields are required!';
      return;
    }
    const dados = this.formulario.value;

    this.serviceCreateUser.enviarMensagem(dados).subscribe({
      next: (resposta) => {
        this.storage.save('token', resposta.acessToken);
        this.storage.save('username', resposta.nomeUsuario);

        this.router.navigate(['/']);
      },

      error: (erro: HttpErrorResponse) => {
        const mensagem = extrairMensagemErro(erro);
        console.log(mensagem);
        this.mensagemErro = mensagem;
        this.changeDetector.markForCheck();
      },
    });
  }
}
