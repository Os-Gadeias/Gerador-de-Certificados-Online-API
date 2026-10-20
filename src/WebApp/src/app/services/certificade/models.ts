export interface ListCertificadesResponse {
  certificados: CertificadeResponse[];
}

export interface EnrollStudentsResponse {
  idsCertificados: string[];
  nomesAlunos: string[];
}

export interface CertificadeResponse {
  nomeAluno: string;
  cursoNome: string;
  cursoDescricao: string;
  cursoCargaHoraria: number;
  cursoData: string;
  status: string;
}
