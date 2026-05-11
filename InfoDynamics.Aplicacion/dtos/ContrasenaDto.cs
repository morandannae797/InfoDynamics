using InfoDynamics.Dominio.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{

    public class ContrasenaResponseDto
    {
        public int IdContrasena { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string Estado { get; set; } = null!;

        public bool EsTemporal { get; set; }

        public int NoUsuario { get; set; }

        public byte[] RowVersion { get; set; } = null!;
    }
    public class ContrasenaUpdateEstadoDto : IConcurrencyDto
    {
        [Required]
        public int IdContrasena { get; set; }

        [Required, RegularExpression("Activa|Inactiva|Historica")]
        public string Estado { get; set; } = null!;

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }

    public class ContrasenaCreateDto
    {
        [Required]
        public int NoUsuario { get; set; }

        [Required, MinLength(8)]
        public string Contrasena { get; set; } = null!;

        public bool EsTemporal { get; set; } = true;
    }
}
