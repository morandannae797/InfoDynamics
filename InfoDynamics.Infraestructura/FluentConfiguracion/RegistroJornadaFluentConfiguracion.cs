using InfoDynamics.Dominio.Entidades;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Registro_FluentConfiguration : IEntityTypeConfiguration<Registro>
    {
        public void Configure(EntityTypeBuilder<Registro> builder)
        {

            builder.ToTable("RegistrosJornada");
            builder.HasKey(e => e.id_registro);

            builder.Property(e => e.horas)
                .HasPrecision(5, 2);
            builder.Property(e => e.RowVersion).IsRowVersion();

            builder.HasOne(d => d.Usuario)
                .WithMany(p => p.RegistrosJornada)
                .HasForeignKey(d => d.no_usuario);
        }
    }
}

