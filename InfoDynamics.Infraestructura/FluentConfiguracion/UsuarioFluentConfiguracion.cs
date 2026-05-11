using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Usuario_FluentConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");

            builder.HasKey(e => e.no_usuario);

            // Importante: Indicar que el ID no lo genera la base de datos (lo asignas tú)
            builder.Property(e => e.no_usuario)
                .ValueGeneratedNever();
            builder.Property(e => e.RowVersion).IsRowVersion();

            builder.Property(e => e.email)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(e => e.email).IsUnique();

            builder.Property(e => e.rol).HasMaxLength(20);


        }
        }
        }
    
