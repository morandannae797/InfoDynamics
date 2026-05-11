using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Vacacion_FluentConfiguration : IEntityTypeConfiguration<Vacacion>
    {
        public void Configure(EntityTypeBuilder<Vacacion> builder)
        {


            builder.HasKey(e => e.id_vacacion);

            builder.Property(e => e.estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            builder.Property(e => e.RowVersion).IsRowVersion();

            // Relación doble con Usuarios
            builder.HasOne(d => d.Solicitante)
                .WithMany(p => p.VacacionesSolicitadas)
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Aprobador)
                .WithMany(p => p.VacacionesAprobadas)
                .HasForeignKey(d => d.id_administrador)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}