using InfoDynamics.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Dominio.Entidades
{
    public class Registro
    {


        public int id_registro { get; set; }

        public DateTime fecha { get; set; }

        public decimal horas { get; set; } // Decimal(4,2)

        public string tipo { get; set; } = null!; // 'Cobrable', 'No cobrable'

        public string estado { get; set; } = null!; // 'Completo', 'Incompleto'

        public int no_usuario { get; set; }

        public int id_periodo    { get; set; }



        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;

        public virtual Periodo Periodo { get; set; } = null!;

    }
}
