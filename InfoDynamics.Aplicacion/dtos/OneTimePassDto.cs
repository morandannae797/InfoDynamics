using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{
    internal class OneTimePassDto
    {
        public int no_usuario { get; set; }

        public string codigo { get; set; } = string.Empty;

        public DateTime fecha_caduca { get; set; }
    }
}
