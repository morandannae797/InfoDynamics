using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class RegistroCreateDto
    {
        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Range(0, 24)]
        public decimal Horas { get; set; }

        [Required]
        public int NoUsuario { get; set; }

        [Required]
        public int PeriodoId { get; set; }

        [Required]
        public string Codigo { get; set; }
    }

    public class RegistroUpdateDto : IConcurrencyDto
    {
        [Required]
        public int RegistroId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }


        [Required]
        [Range(0, 24)]
        public decimal Horas { get; set; }

        [Required]
        public int PeriodoId { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }
    public class RegistroResponseDto
    {
        public int RegistroId { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Horas { get; set; }

        public int NoUsuario { get; set; }

        public int PeriodoId { get; set; }

        public string  Codigo { get; set; }

        public byte[] RowVersion { get; set; } = null!;
    }
}
