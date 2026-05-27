using InfoDynamics.Dominio.Entidades;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Proyecto_FluentConfiguration : IEntityTypeConfiguration<Proyecto>
    {
        public void Configure(EntityTypeBuilder<Proyecto> builder)
        {
            builder.HasKey(e => e.codigo).HasName("PK__Proyecto__F38AD81D4DE706FE");

            builder.ToTable("Proyecto");

            builder.HasIndex(e => e.codigo, "UQ__Proyecto__40F9A2068ED98A87").IsUnique();

            builder.Property(e => e.es_cobrable)
                .HasMaxLength(15)
                .IsUnicode(false);
            builder.Property(e => e.codigo)
                .HasMaxLength(4)
                .IsUnicode(false);

            builder.HasOne(d => d.id_empresaNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.id_empresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Proyecto_Empresa");

        }
    }
}

