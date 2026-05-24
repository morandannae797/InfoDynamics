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

            builder.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            builder.Property(e => e.estado)
                .HasMaxLength(15)
                .IsUnicode(false);


            builder.HasOne(d => d.id_administradorNavigation).WithMany(p => p.Vacacionid_administradorNavigations)
                .HasForeignKey(d => d.no_usuario)
                .HasConstraintName("FK_Vacacion_Administrador");

            builder.HasOne(d => d.no_usuarioNavigation).WithMany(p => p.Vacacionno_usuarioNavigations)
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacacion_Usuario");
        }


    }
}