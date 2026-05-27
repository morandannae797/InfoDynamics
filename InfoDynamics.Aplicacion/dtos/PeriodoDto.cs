using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{

    /*
    public class PeriodoCreateDto
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required, RegularExpression("Abierto|Cerrado")]
        public string Estado { get; set; } = null!;
    }

    public class PeriodoUpdateDto : IConcurrencyDto
    {
        [Required]
        public int PeriodoId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required, RegularExpression("Abierto|Cerrado")]
        public string Estado { get; set; } = null!;

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }

    */
    public class PeriodoResponseDto
    {
        public int PeriodoId { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public string Estado { get; set; } = null!;

        public byte[] RowVersion { get; set; } = null!;
    }
}
