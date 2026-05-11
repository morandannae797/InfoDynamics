using System;
using System.Collections.Generic;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Infraestructura.Contexto;
using InfoDynamics.Infraestructura.FluentConfiguracion;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.Infraestructura.Contexto;

public partial class EmployeesDbContext : DbContext
{
    public EmployeesDbContext()
    {
    }

    public EmployeesDbContext(DbContextOptions<EmployeesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Periodo> Periodos { get; set; }

    public virtual DbSet<Registro> Registro { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual  DbSet<Vacacion> Vacacion { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Empresa_FluentConfiguration());
 modelBuilder.ApplyConfiguration(new FluentConfiguracion.Vacacion_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Periodo_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Registro_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Usuario_FluentConfiguration());


        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}