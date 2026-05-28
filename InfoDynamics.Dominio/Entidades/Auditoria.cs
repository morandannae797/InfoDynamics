using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Dominio.Entidades
{
    public class Auditoria
    {
        public int id_auditoria { get; set; }

        public int id_registro { get; set; }

        public DateOnly fecha { get; set; }

        public decimal horas { get; set; }

        public int no_usuario { get; set; }

        public int id_periodo { get; set; }

        public string codigo { get; set; } = null!;

        public string accion { get; set; } = null!;

        public int usuario_accion { get; set; }

        public DateTime fecha_accion { get; set; }

        public virtual Usuario no_usuarioNavigation { get; set; } = null!;
   
        public virtual Usuario usuario_accionNavigation { get; set; } = null!;

        public virtual Periodo id_periodoNavigation { get; set; } = null!;
    }
}
