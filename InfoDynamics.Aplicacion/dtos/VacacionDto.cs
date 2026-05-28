using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class VacacionDto
    {
        public class VacacionCreateDto
        {
            [Required]
            public DateTime FechaInicio { get; set; }

            [Required]
            public DateTime FechaFin { get; set; }

            public int SolicitanteId { get; set; }
        }

        public class VacacionResponseDto
        {
            public int VacacionId { get; set; }

            public DateTime FechaInicio { get; set; }

            public DateTime FechaFin { get; set; }

            public string Estado { get; set; } = null!;

            public int SolicitanteId { get; set; }
        }

        public class VacacionAprobacionDto
        {
            [Required]
            [RegularExpression("Aprobada|Rechazada", ErrorMessage = "El estado debe ser 'Aprobada' o 'Rechazada'.")]
            public string EstadoDecision { get; set; } = null!;
        }
    }
}