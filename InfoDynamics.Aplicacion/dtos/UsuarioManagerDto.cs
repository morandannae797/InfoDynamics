using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using InfoDynamics.Dominio.interfaces;




namespace InfoDynamics.Aplicacion.dtos
{


        public class UsuarioManagerDto : IConcurrencyDto
        {
            [Required]
            public int NoUsuario { get; set; }

            [Required]
            public int NoUsuarioManager { get; set; }

        }
    }

