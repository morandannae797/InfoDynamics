using InfoDynamics.Dominio.Entidades;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Registro_FluentConfiguration : IEntityTypeConfiguration<Registro>
    {
        public void Configure(EntityTypeBuilder<Registro> builder)
        {

            builder.HasKey(e => e.id_registro).HasName("PK__Registro__48155C1FE3B91FF2");

            builder.ToTable("Registro");

            builder.Property(e => e.horas).HasColumnType("decimal(5, 2)");

            builder.HasOne(d => d.id_periodoNavigation).WithMany(p => p.Registros)
                .HasForeignKey(d => d.id_periodo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Periodo");

            builder.HasOne(d => d.id_proyectoNavigation).WithMany(p => p.Registros)
                .HasForeignKey(d => d.codigo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Proyecto");

            builder.HasOne(d => d.no_usuarioNavigation).WithMany(p => p.Registros)
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registro_Usuario");
        }
    }
}

