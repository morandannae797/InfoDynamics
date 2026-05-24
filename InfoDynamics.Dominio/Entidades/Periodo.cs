using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Dominio.Entidades
{
    public class Periodo
    {
        public int id_periodo { get; set; }

        public DateOnly fecha_inicio { get; set; } 

        public DateOnly fecha_fin { get; set; }

        public string estado { get; set; } = null!;

        public byte[] RowVersion { get; set; } = null!;

        public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
    }
}
