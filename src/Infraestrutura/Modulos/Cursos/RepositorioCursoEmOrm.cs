using GeradorCertificados.Dominio.Compartilhado;
using GeradorCertificados.Dominio.Modulos.ModuloCurso;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;

namespace GeradorCertificados.Infraestrutura.Modulos.Cursos;

public class RepositorioCursoEmOrm(GeradorCertificadosDbContext dbContext) : RepositorioBaseEmOrm<Curso>(dbContext), IRepositorioCurso
{

}