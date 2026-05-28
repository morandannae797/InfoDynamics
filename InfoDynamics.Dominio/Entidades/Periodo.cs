using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Dominio.Entidades
{
    public class Periodo
    {
        public int id_periodo { get; set; }

        public DateTime fecha_inicio { get; set; } 

        public DateTime fecha_fin { get; set; }

        public string estado { get; set; } = null!;


        public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
    }
}
