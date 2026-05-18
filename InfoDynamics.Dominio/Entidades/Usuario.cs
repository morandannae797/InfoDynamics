using InfoDynamics.Dominio.Entidades;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace InfoDynamics.Dominio.Entidades
{

    public partial class Usuario

    {
        public int? id_administrador { get; set; }
        public int no_usuario { get; set; }

        public string nombre { get; set; } = null!;

        public string ap_paterno { get; set; } = null!;

        public string? ap_materno { get; set; }

        public string email { get; set; } = null!;

        public string rol { get; set; } = null!;

        public string estado_cuenta { get; set; } = null!;

        public byte[] RowVersion { get; set; } = null!;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<Contrasena> Contrasenas { get; set; } = new List<Contrasena>();

        public virtual ICollection<Preguntum> Pregunta { get; set; } = new List<Preguntum>();

        public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

       public virtual ICollection<Vacacion> Vacacionid_administradorNavigations { get; set; } = new List<Vacacion>();

        public virtual ICollection<Vacacion> Vacacionno_usuarioNavigations { get; set; } = new List<Vacacion>();


    }
}
