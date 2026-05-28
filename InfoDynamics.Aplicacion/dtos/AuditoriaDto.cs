using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class AuditoriaDto
    {
        public class AuditoriaCreateDto
        {
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
        }

        public class AuditoriaResponseDto
        {
            public int IdAuditoria { get; set; }

            public int IdRegistro { get; set; }

            public DateTime Fecha { get; set; }

            public decimal Horas { get; set; }

            public int NoUsuario { get; set; }

            public int IdPeriodo { get; set; }

            public string Codigo { get; set; } = null!;

            public string Accion { get; set; } = null!;

            public int UsuarioAccion { get; set; }

            public DateTime FechaAccion { get; set; }
        }
    }
}