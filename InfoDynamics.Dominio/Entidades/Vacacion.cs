using InfoDynamics.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoDynamics.Dominio.Entidades
{
    public class Vacacion
    {
        public int id_vacacion { get; set; }

        public DateOnly fecha_inicio { get; set; }

        public DateOnly fecha_fin { get; set; }

        public string estado { get; set; } = null!;

        public int no_usuario { get; set; }

        public byte[] RowVersion { get; set; } = null!;

        public virtual Usuario? id_administradorNavigation { get; set; }

        public virtual Usuario no_usuarioNavigation { get; set; } = null!;

    }
}
