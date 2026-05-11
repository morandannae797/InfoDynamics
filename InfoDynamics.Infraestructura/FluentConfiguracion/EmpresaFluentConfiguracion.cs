using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class Empresa_FluentConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
       
    
            builder.HasKey(e => e.id_empresa).HasName("id_empresa");

            builder.ToTable("Empresa");

            builder.Property(e => e.id_empresa).HasColumnName("id_empresa");
            builder.Property(e => e.descripcion).HasMaxLength(200)
                .HasColumnName("descripcion");
            builder.Property(e => e.RowVersion).IsRowVersion();
        
        builder.Property(e => e.nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        }

          
        
        }
    }


