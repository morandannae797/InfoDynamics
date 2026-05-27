using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using InfoDynamics.Dominio.interfaces;




namespace InfoDynamics.Aplicacion.dtos

{

        public class UsuarioManagerResponseDto
        {
            [Required]
            public int NoUsuario { get; set; }

            [Required]
            public int NoUsuarioManager { get; set; }

            [Required]
            public byte[] RowVersion { get; set; } = null!;
        }

        public class UsuarioManagerCreateDto
        {
            [Required]
            public int NoUsuario { get; set; }

            [Required]
            public int NoUsuarioManager { get; set; }
        }

        public class UsuarioManagerUpdateDto : IConcurrencyDto
        {
            [Required]
            public int NoUsuario { get; set; }

            [Required]
            public int NoUsuarioManager { get; set; }

            [Required]
            public byte[] RowVersion { get; set; } = null!;
        }
    }

