using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class VacacionDto
    {
        public class VacacionCreateDTO
        {
            [Required]
            public DateTime FechaInicio { get; set; }

            [Required]
            public DateTime FechaFin { get; set; }

            [Required]
            public int SolicitanteId { get; set; }
        }

        public class VacacionResponseDTO
        {
            public int VacacionId { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public string EstadoAprobacion { get; set; } = null!;
            public string NombreSolicitante { get; set; } = null!;
            public string? NombreAprobador { get; set; }

            public byte[] RowVersion { get; set; } = null!;
        }
        public class VacacionAprobacionDTO
        {
            [Required]
            public int VacacionId { get; set; }

            [Required]
            public int AprobadorId { get; set; }

            [Required]
            [RegularExpression("Aprobada|Rechazada", ErrorMessage = "El estado debe ser 'Aprobada' o 'Rechazada'")]
            public string EstadoDecision { get; set; } = null!;

            public byte[]? RowVersion { get; set; } = null!;
        }
    }

}
