using InfoDynamics.Dominio.Entidades;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace InfoDynamics.Dominio.Entidades
{

    public partial class Usuario

    {
        public int no_usuario { get; set; }

        public string nombre { get; set; } = null!;

        public string ap_paterno { get; set; } = null!;

        public string? ap_materno { get; set; }

        public string email { get; set; } = null!;

        public bool es_manager { get; set; }

        public bool estado_cuenta { get; set; } = true;

        public string contrasena_hash { get; set; }  = null!;

        public bool debe_cambiar_pass { get; set; }

        public int intentos { get; set; }

        public DateTime? hora_bloqueo { get; set; }

        public byte[] RowVersion { get; set; } = null!;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<Contrasena> Contrasenas { get; set; } = new List<Contrasena>();

        public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

       public virtual ICollection<Vacacion> Vacacionid_administradorNavigations { get; set; } = new List<Vacacion>();

        public virtual ICollection<Vacacion> Vacacionno_usuarioNavigations { get; set; } = new List<Vacacion>();


    }
}
