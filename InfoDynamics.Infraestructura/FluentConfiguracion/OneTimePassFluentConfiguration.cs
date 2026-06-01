using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Infraestructura.FluentConfiguracion
{
    internal class OneTimePassFluentConfiguration : IEntityTypeConfiguration<OneTimePass>
    {
        public void Configure(EntityTypeBuilder<OneTimePass> builder)
        {
            builder.ToTable("OneTimePass");

            builder.HasKey(e => e.no_usuario);

            builder.Property(e => e.codigo)
                .IsRequired()
                .HasMaxLength(6)
                .HasColumnType("varchar(6)");

            builder.Property(e => e.fecha_caduca)
                .HasColumnType("datetime2");

            builder.HasOne(e => e.Usuario)
                .WithOne()
                .HasForeignKey<OneTimePass>(e => e.no_usuario)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
