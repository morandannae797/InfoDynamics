using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{


    public class PeriodoDto
    {
        public int PeriodoId { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }


        [Required, RegularExpression("Abierto|Cerrado")]
        public string Estado { get; set; } = null!;


    }
}
