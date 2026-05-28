using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Vacacion_FluentConfiguration : IEntityTypeConfiguration<Vacacion>
    {
        public void Configure(EntityTypeBuilder<Vacacion> builder)
        {


            builder.HasKey(e => e.id_vacacion).HasName("PK__Vacacion__726C3EFE67A99459");

            builder.ToTable("Vacacion");

            builder.Property(e => e.estado)
                .HasMaxLength(15)
                .IsUnicode(false);


        }


    }
}