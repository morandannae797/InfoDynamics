
using InfoDynamics.Dominio.Entidades;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Preguntum_FluentConfiguration : IEntityTypeConfiguration<Preguntum>
    {
        public void Configure(EntityTypeBuilder<Preguntum> builder)
        {

            builder.HasKey(e => e.id_pregunta).HasName("PK__Pregunta__6867FFA44BD88D48");

            builder.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            builder.Property(e => e.pregunta)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.respuesta)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.HasOne(d => d.no_usuarioNavigation).WithMany(p => p.Pregunta)
                .HasForeignKey(d => d.no_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pregunta_Usuario");

        }
    }
}

