using InfoDynamics.Dominio.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{
    public class PreguntaCreateDto
    {
        [Required, StringLength(255)]
        public string Pregunta { get; set; } = null!;

        [Required, StringLength(255)]
        public string Respuesta { get; set; } = null!;

        [Required]
        public int NoUsuario { get; set; }
    }

    public class PreguntaUpdateDto : IConcurrencyDto
    {
        [Required]
        public int IdPregunta { get; set; }

        [Required, StringLength(255)]
        public string Pregunta { get; set; } = null!;

        [Required, StringLength(255)]
        public string Respuesta { get; set; } = null!;

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }

    public class PreguntaResponseDto
    {
        public int IdPregunta { get; set; }

        public string Pregunta { get; set; } = null!;

        public string Respuesta { get; set; } = null!;

        public int NoUsuario { get; set; }

        public string? NombreUsuario { get; set; }

        public byte[] RowVersion { get; set; } = null!;
    }
}
