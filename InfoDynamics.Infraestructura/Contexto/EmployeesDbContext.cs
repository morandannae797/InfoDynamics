
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Infraestructura.Contexto;
using InfoDynamics.Infraestructura.FluentConfiguracion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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
    public virtual DbSet<Auditoria> Auditorias { get; set; }
    public virtual DbSet<Periodo> Periodos { get; set; }

    public virtual DbSet<Registro> Registro { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual  DbSet<Vacacion> Vacacion { get; set; }
    public virtual DbSet<HistorialContrasena> Contrasenas { get; set; }
    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<Usuario_manager> UsuarioManagers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

       
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Auditoria_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.UsuarioManager_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Empresa_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Vacacion_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Periodo_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Registro_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Usuario_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Proyecto_FluentConfiguration());
        modelBuilder.ApplyConfiguration(new FluentConfiguracion.Contrasena_FluentConfiguration());






        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}