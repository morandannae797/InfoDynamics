  using InfoDynamics.Dominio.Entidades;


    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfoDynamics.Infraestructura.FluentConfiguracion
{


    public class Periodo_FluentConfiguration : IEntityTypeConfiguration<Periodo>
    {
        public void Configure(EntityTypeBuilder<Periodo> builder)
        {
            builder.HasKey(e => e.id_periodo).HasName("PK__Periodo__801188B75CDD8DF2");

            builder.ToTable("Periodo");

            builder.Property(e => e.estado)
                .HasMaxLength(15)
                .IsUnicode(false);
        }
    }
}

