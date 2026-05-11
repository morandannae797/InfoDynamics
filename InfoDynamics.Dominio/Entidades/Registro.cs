using InfoDynamics.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;


namespace InfoDynamics.Dominio.Entidades
{
    public class Registro
    {


        public int id_registro { get; set; }

        public DateTime fecha { get; set; }
        public TimeOnly hora_inicio { get; set; }

        public TimeOnly hora_fin { get; set; }
        public decimal horas { get; set; } // Decimal(4,2)



        public int no_usuario { get; set; }
        public int id_proyecto { get; set; }

        public int id_periodo    { get; set; }



        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public virtual Periodo id_periodoNavigation { get; set; } = null!;

        public virtual Proyecto id_proyectoNavigation { get; set; } = null!;

        public virtual Usuario no_usuarioNavigation { get; set; } = null!;

    }
}
