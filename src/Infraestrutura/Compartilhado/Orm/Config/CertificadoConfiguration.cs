using System.Runtime.Serialization;
using GeradorCertificados.Dominio.Modulos.ModuloCertificado;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificados.Infraestrutura.Compartilhado.Orm.Config;

public class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {

        //Talvez seja necessário aplicar o IEntidadeDoUsuarioAqui
        //Para a navegacao
        
        builder.ToTable("TBCertificado");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.NomeAluno)
            .IsRequired();

        builder.HasOne(c => c.Curso)
            .WithMany(c => c.Certificados)
            .HasForeignKey(c => c.Id);
    }

}
