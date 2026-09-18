using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;

namespace GeradorCertificados.Infraestrutura.Modulos.Certificados;

public class RepositorioCertificadoOrm(GeradorCertificadosDbContext dbContext) :
    RepositorioBaseEmOrm<Certificado>(dbContext), IRepositorioCertificado
{
}
