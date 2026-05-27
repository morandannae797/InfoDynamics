using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Empresa_FluentConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {


            builder.HasKey(e => e.id_empresa).HasName("PK__Empresa__4A0B7E2CA32B022E");

            builder.ToTable("Empresa");

           // builder.Property(e => e.RowVersion)
            //    .IsRowVersion()
            //    .IsConcurrencyToken();
            builder.Property(e => e.nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        }

          
        
        }
    }


