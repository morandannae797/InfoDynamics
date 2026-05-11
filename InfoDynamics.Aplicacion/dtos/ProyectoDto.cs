using InfoDynamics.Dominio.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{

    public class ProyectoCreateDto
    {
        [Required, StringLength(4)]
        public string Codigo { get; set; } = null!;

        [Required, RegularExpression("Cobrable|No cobrable")]
        public string Categoria { get; set; } = null!;

        [Required]
        public int IdEmpresa { get; set; }
    }

    public class ProyectoUpdateDto : IConcurrencyDto
    {
        [Required]
        public int IdProyecto { get; set; }

        [Required, StringLength(4)]
        public string Codigo { get; set; } = null!;

        [Required, RegularExpression("Cobrable|No cobrable")]
        public string Categoria { get; set; } = null!;

        [Required]
        public int IdEmpresa { get; set; }

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }
    public class ProyectoResponseDto
    {
        public int IdProyecto { get; set; }

        public string Codigo { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public int IdEmpresa { get; set; }

        public string? NombreEmpresa { get; set; }

        public byte[] RowVersion { get; set; } = null!;
    }
}
