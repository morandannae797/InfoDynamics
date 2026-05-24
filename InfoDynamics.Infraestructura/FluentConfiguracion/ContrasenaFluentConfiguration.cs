using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Contrasena_FluentConfiguration : IEntityTypeConfiguration<HistorialContrasena>
    {
        public void Configure(EntityTypeBuilder<HistorialContrasena> builder)
        {
            builder.HasKey(e => e.id_historial);

            builder.ToTable("Historial_contrasenas");

            builder.Property(e => e.contrasena_hash)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.Property(e => e.fecha_registro)
                .HasColumnType("datetime");

            builder.Property(e => e.es_temporal);

            builder.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.HasOne(d => d.no_usuarioNavigation)
                .WithMany(p => p.Contrasenas)
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contrasena_Usuario");
        }
    }
}


    

