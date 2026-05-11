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

        public string ap_maternos { get; set; } = null!;

        public string email { get; set; } = null!;

        public string contrasena { get; set; } = null!; 

        public string rol { get; set; } = null!;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<Registro> RegistrosJornada { get; set; } = new List<Registro>();

    public virtual ICollection<Vacacion> VacacionesSolicitadas { get; set; } = new List<Vacacion>();

    public virtual ICollection<Vacacion> VacacionesAprobadas { get; set; } = new List<Vacacion>();

} 
    }
