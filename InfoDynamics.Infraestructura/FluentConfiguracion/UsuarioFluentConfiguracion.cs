using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Usuario_FluentConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(e => e.no_usuario).HasName("PK__Usuario__D61D9F570C308F1C");

            builder.ToTable("Usuario");

            builder.HasIndex(e => e.email, "UQ__Usuario__AB6E6164F61F8696").IsUnique();

            builder.Property(e => e.no_usuario).ValueGeneratedNever();
            builder.Property(e => e.RefreshToken)
                .HasMaxLength(500)
                .IsUnicode(false);
            builder.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
            builder.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            builder.Property(e => e.ap_materno)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.ap_paterno)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.email)
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.estado_cuenta)
                .HasMaxLength(15)
                .IsUnicode(false);
            builder.Property(e => e.nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.es_manager)
                .HasMaxLength(15)
                .IsUnicode(false);


        }
        }
        }
    
