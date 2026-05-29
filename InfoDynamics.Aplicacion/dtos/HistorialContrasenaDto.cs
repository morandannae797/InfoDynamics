using InfoDynamics.Dominio.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{


    public class  HistorialContrasenaCreateDto
    {
        [Required]
        public int NoUsuario { get; set; }

        [Required]

        [MinLength(12)]
        public string Contrasena { get; set; } = null!;

    }

    public class HistorialContrasenaAmbosDto : IConcurrencyDto
    {
        [Required]
        public int IdHistorial { get; set; }

        [Required]
        public DateTime Fecharegistro { get; set; }


        [Required]
        public int NoUsuario { get; set; } 

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }
  
}
