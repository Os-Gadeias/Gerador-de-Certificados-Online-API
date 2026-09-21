using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using GeradorCertificados.Infraestrutura.Compartilhado.Orm;

namespace GeradorCertificados.Infraestrutura.Modulos.Certificados;

public class RepositorioCertificadoEmOrm(
    GeradorCertificadosDbContext dbContext, IProvedorDeUsuario provedorDeUsuario
    ) : RepositorioBaseEmOrm<Certificado>(dbContext, provedorDeUsuario), IRepositorioCertificado
{

}
