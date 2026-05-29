using System;
using System.Collections.Generic;
using System.Text;
using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class MEmpleadoDto
    {
        public int IdUsuario { get; set; }

        public string NombreEmpleado { get; set; }

        public decimal TotalHoras { get; set; }
    }
}
