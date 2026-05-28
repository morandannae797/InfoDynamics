using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Dominio.Entidades
{
    public partial class Auditoria
    {
        public int id_auditoria { get; set; }

        public int id_registro { get; set; }

        public int id_periodo { get; set; }

        public string codigo { get; set; }

        public DateTime fecha { get; set; }

        public float horas { get; set; }

        public String accion { get; set; }
   

    }
}
