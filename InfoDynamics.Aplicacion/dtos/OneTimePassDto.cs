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

    // METODOS PARA CONTROLLER EN USUARIO
    public class SolicitarCodigoDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ValidarCodigoDto
    {
        public int NoUsuario { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }
}
