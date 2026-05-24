using InfoDynamics.Dominio.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{

    public class ProyectoCreateDto
    {
        [Required, StringLength(7)]
        public string Codigo { get; set; } = null!;

        public bool EsCobrable { get; set; } 

        [Required]
        public int IdEmpresa { get; set; }
    }

    public class ProyectoUpdateDto : IConcurrencyDto
    {

        [Required, StringLength(7)]
        public string Codigo { get; set; } = null!;


        public bool EsCobrable { get; set; }

        [Required]
        public int IdEmpresa { get; set; }

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }
    public class ProyectoResponseDto
    {
        [Required, StringLength(7)]
        public string Codigo { get; set; } = null!;

        public bool EsCobrable { get; set; } 

        public int IdEmpresa { get; set; }

        public byte[] RowVersion { get; set; } = null!;
    }
}
