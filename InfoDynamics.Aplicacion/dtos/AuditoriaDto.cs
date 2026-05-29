using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class AuditoriaDto
    {

        [Required]
        public int IdAuditoria { get; set; }

        [Required]
        public int IdRegistro { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public decimal Horas { get; set; }

        [Required]
        public int NoUsuario { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        [Required]
        public string Codigo { get; set; } = null!;

        [Required]
        public string Accion { get; set; } = null!;

        [Required]
        public int UsuarioAccion { get; set; }

        [Required]

        public DateTime FechaAccion { get; set; }
    }
}