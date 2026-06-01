using System;

namespace InfoDynamics.Dominio.Entidades
{
    public class OneTimePass
    {
        public int no_usuario { get; set; }
        public string codigo { get; set; }
        public DateTime? fecha_caduca { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}