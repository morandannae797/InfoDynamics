
using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Contrasena_FluentConfiguration : IEntityTypeConfiguration<Contrasena>
    {
        public void Configure(EntityTypeBuilder<Contrasena> builder)
        {


            builder.HasKey(e => e.id_contrasena).HasName("PK__Contrase__C01E9A3A66B395E4");

            builder.ToTable("Contrasena");

            builder.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            builder.Property(e => e.contrasena)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.estado)
                .HasMaxLength(15)
                .IsUnicode(false);
            builder.Property(e => e.fecha_creacion).HasColumnType("datetime");

            builder.HasOne(d => d.no_usuarioNavigation).WithMany(p => p.Contrasenas)
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contrasena_Usuario");
        }



    }
}


