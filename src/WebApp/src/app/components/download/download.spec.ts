import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ProvideUser } from '../../auth/provideUser';
import { ServiceDeleteCourse } from '../../services/course/deleteCourse/serviceDeleteCourse';
import { ServiceListCourses } from '../../services/course/listCourses/serviceListCourses';
import { ListCourseResponse } from '../../services/course/models/Course';
import { ServiceListCertificades } from '../../services/certificade/serviceListCertificades';
import { ListCertificadesResponse } from '../../services/certificade/models';
import { Download } from './download';

describe('Download: sincronização de cursos excluídos', () => {
  let component: Download;
  let http: HttpTestingController;
  let deleteService: ServiceDeleteCourse;
  let listCourses: ReturnType<typeof vi.fn>;
  let listCertificates: ReturnType<typeof vi.fn>;
  const course: ListCourseResponse = {
    id: 'curso-1',
    nome: 'Curso teste',
    descricao: null,
    cargaHoraria: 20,
    dataConclusao: '2026-11-10',
    qntdCertificados: 1,
  };

  beforeEach(() => {
    vi.useFakeTimers();
    listCourses = vi.fn().mockReturnValue(of([course]));
    listCertificates = vi.fn().mockReturnValue(of({ certificados: [] }));
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ProvideUser, useValue: { isLoggedIn: signal(true) } },
        { provide: ServiceListCourses, useValue: { enviarMensagem: listCourses } },
        { provide: ServiceListCertificades, useValue: { enviarMensagem: listCertificates } },
      ],
    });
    component = TestBed.runInInjectionContext(() => new Download());
    deleteService = TestBed.inject(ServiceDeleteCourse);
    http = TestBed.inject(HttpTestingController);
    TestBed.tick();
    vi.advanceTimersByTime(0);
  });

  afterEach(() => {
    http.verify();
    TestBed.resetTestingModule();
    vi.useRealTimers();
  });

  it('remove imediatamente o curso após a API confirmar a exclusão', () => {
    expect(component['courses']()).toHaveLength(1);
    deleteService.deleteCourse(course.id).subscribe();
    http.expectOne(`https://localhost:7094/api/curso/${course.id}`).flush(true);
    expect(component['courses']()).toEqual([]);
  });

  it('mantém o curso quando a exclusão falha', () => {
    deleteService.deleteCourse(course.id).subscribe({ error: () => {} });
    http.expectOne(`https://localhost:7094/api/curso/${course.id}`).flush(null, {
      status: 500,
      statusText: 'Erro',
    });
    expect(component['courses']()).toHaveLength(1);
  });

  it('limpa a seleção e ignora respostas de certificados do curso excluído', () => {
    const pendingCertificates = new Subject<ListCertificadesResponse>();
    listCertificates.mockReturnValue(pendingCertificates);
    component['selectCourse'](course.id);
    TestBed.tick();
    vi.advanceTimersByTime(0);

    deleteService.deleteCourse(course.id).subscribe();
    http.expectOne(`https://localhost:7094/api/curso/${course.id}`).flush(true);
    pendingCertificates.next({ certificados: [] });

    expect(component['selectedCourse']()).toBeNull();
    expect(component['certificades']()).toBeNull();
    expect(component['loadingCertificades']()).toBe(false);
    expect(component['certificatesReady']()).toBe(false);
    TestBed.tick();
    expect(pendingCertificates.observed).toBe(false);
  });

  it('remove cursos excluídos em outra sessão na atualização periódica', () => {
    component['selectCourse'](course.id);
    TestBed.tick();
    listCourses.mockReturnValue(of([]));
    vi.advanceTimersByTime(3000);

    expect(component['courses']()).toEqual([]);
    expect(component['selectedCourse']()).toBeNull();
  });

  it('cancela a listagem anterior à exclusão para não restaurar dados antigos', () => {
    const oldResponse = new Subject<ListCourseResponse[]>();
    listCourses.mockReturnValue(oldResponse);
    vi.advanceTimersByTime(3000);
    expect(oldResponse.observed).toBe(true);

    deleteService.deleteCourse(course.id).subscribe();
    http.expectOne(`https://localhost:7094/api/curso/${course.id}`).flush(true);
    expect(oldResponse.observed).toBe(false);
    oldResponse.next([course]);
    expect(component['courses']()).toEqual([]);
  });
});
