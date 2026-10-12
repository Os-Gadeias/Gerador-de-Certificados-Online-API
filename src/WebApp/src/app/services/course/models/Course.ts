export interface CreateCourseRequest {
  nome: string;
  descricao: string | null;
  cargaHoraria: number;
  dataConclusao: string;
}
export interface CreateCourseResponse {
  id: string;
}
