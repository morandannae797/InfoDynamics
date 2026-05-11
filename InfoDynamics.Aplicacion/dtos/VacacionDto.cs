using InfoDynamics.Dominio.interfaces;
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

            [Required]
            public int SolicitanteId { get; set; }
        }

        public class VacacionResponseDto 
        {
            public int VacacionId { get; set; }

            public DateTime FechaSolicito { get; set; }

            public DateTime? FechaAprobo { get; set; }

            public DateTime FechaInicio { get; set; }

            public DateTime FechaFin { get; set; }

            public string EstadoAprobacion { get; set; } = null!;

            public int SolicitanteId { get; set; }

            public string NombreSolicitante { get; set; } = null!;

            public int? AprobadorId { get; set; }

            public string? NombreAprobador { get; set; }

            public byte[] RowVersion { get; set; } = null!;
        }

        public class VacacionAprobacionDto : IConcurrencyDto
        {
            [Required]
            public int VacacionId { get; set; }

            [Required]
            public int AprobadorId { get; set; }

            [Required]
            [RegularExpression("Aprobada|Rechazada", ErrorMessage = "El estado debe ser 'Aprobada' o 'Rechazada'.")]
            public string EstadoDecision { get; set; } = null!;

            [Required]
            public byte[] RowVersion { get; set; } = null!;
        }
    }

}
