using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class RegistroCreateDto
    {
        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Required]
        [Range(0, 24)]
        public decimal Horas { get; set; }

        [Required]
        public int NoUsuario { get; set; }

        [Required]
        public int PeriodoId { get; set; }

        [Required]
        public int ProyectoId { get; set; }
    }

    public class RegistroUpdateDto : IConcurrencyDto
    {
        [Required]
        public int RegistroId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Required]
        [Range(0, 24)]
        public decimal Horas { get; set; }

        [Required]
        public int PeriodoId { get; set; }

        [Required]
        public int ProyectoId { get; set; }

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }
    public class RegistroResponseDto
    {
        public int RegistroId { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public decimal Horas { get; set; }

        public int NoUsuario { get; set; }

        public string? NombreUsuario { get; set; }

        public int PeriodoId { get; set; }

        public int ProyectoId { get; set; }

        public string? CodigoProyecto { get; set; }

        public byte[] RowVersion { get; set; } = null!;
    }
}
