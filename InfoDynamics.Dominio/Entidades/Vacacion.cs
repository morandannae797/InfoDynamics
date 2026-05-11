using InfoDynamics.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoDynamics.Dominio.Entidades
{
    public class Vacacion
    {
     [Key]   public int id_vacacion { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }
        public DateTime fecha_solicito { get; set; }
        public DateTime fecha_aprobo { get; set; }

        public string estado { get; set; } = "Pendiente"; // 'Pendiente', 'Aprobada', 'Rechazada'

        public int no_usuario { get; set; }

        public int? id_administrador { get; set; } // nullable

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        [ForeignKey("no_usuario")]
        public virtual Usuario Solicitante { get; set; } = null!;

        [ForeignKey("id_administrador")]
        public virtual Usuario? Aprobador { get; set; }

        // Propiedades de navegación

    }
}
