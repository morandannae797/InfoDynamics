using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Auditoria_FluentConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.HasKey(e => e.id_auditoria);

            builder.ToTable("Auditoria");

            builder.Property(e => e.fecha);

            builder.Property(e => e.horas)
                .HasColumnType("decimal(10,2)");

            builder.Property(e => e.codigo)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.accion)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.fecha_accion)
                .HasColumnType("datetime");

            builder.HasOne(d => d.no_usuarioNavigation)
                .WithMany()
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auditoria_Usuario");

            builder.HasOne(d => d.usuario_accionNavigation)
                .WithMany()
                .HasForeignKey(d => d.usuario_accion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auditoria_UsuarioAccion");

            builder.HasOne(d => d.id_periodoNavigation)
                .WithMany()
                .HasForeignKey(d => d.id_periodo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auditoria_Periodo");
        }
    }
}