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

        builder.Property(c => c.NomeAluno)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CaminhoPdf)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>();


        builder.HasOne(c => c.Curso)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

}
