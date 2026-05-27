using InfoDynamics.Dominio.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{

    public class ProyectoDto
    {
        [Required, StringLength(7)]
        public string Codigo { get; set; } = null!;

        public bool EsCobrable { get; set; } 

        [Required]
        public int IdEmpresa { get; set; }
    }

 
}
