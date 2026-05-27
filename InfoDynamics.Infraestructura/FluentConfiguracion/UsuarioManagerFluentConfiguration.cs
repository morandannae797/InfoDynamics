using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{

       internal class UsuarioManager_FluentConfiguration : IEntityTypeConfiguration<Usuario_manager>
        {
            public void Configure(EntityTypeBuilder<Usuario_manager> builder)
            {
                builder.ToTable("Usuario_manager");

                builder.HasKey(e => new { e.no_usuario_manager, e.no_usuario });

                builder.Property(e => e.no_usuario_manager)
                    .HasColumnName("no_usuario_manager");

                builder.Property(e => e.no_usuario)
                    .HasColumnName("no_usuario");
            }
        }
    }

