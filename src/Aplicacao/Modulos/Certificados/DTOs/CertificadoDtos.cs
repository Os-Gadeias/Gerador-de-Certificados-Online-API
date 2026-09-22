using GeradorCertificados.Dominio.Modulos.ModuloCertificado;

namespace GeradorCertificados.Aplicacao.Modulos.Certificados.DTOs;

public sealed record DownloadCertificadosResponse(
    byte[] Arquivo,
    string NomeArquivo
);

public sealed record SelecionarCursoResponse(
    StatusCurso StatusCurso
);

public sealed record ListarCertificadoDto(
    string NomeAluno,
    string CursoNome,
    string? CursoDescricao,
    int CursoCargaHoraria,
    DateTime CursoData,
    StatusGeracaoCertificado Status
);
public sealed record SolicitarListaCertificadoResponse(
   List<ListarCertificadoDto> Certificados
);