  using InfoDynamics.Dominio.Entidades;


    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfoDynamics.Infraestructura.FluentConfiguracion
{


    public class Periodo_FluentConfiguration : IEntityTypeConfiguration<Periodo>
    {
        public void Configure(EntityTypeBuilder<Periodo> builder)
        {
            builder.ToTable("Periodos");
            builder.HasKey(e => e.id_periodo);


            builder.Property(e => e.estado)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasMany(e => e.Registros)
                .WithOne(r => r.Periodo)
                .HasForeignKey(r => r.id_periodo);
            builder.Property(e => e.RowVersion).IsRowVersion();
        }
    }
}

