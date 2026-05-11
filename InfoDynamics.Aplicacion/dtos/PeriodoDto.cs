using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class PeriodoDto
    {
        public int PeriodoID { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
        [Required]
        public string Estado { get; set; }

        public byte[]? RowVersion { get; set; } = null!;
    }

}
