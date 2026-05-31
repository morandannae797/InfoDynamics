using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{
    public class loginDto
    {

        [Required]
        public string identificador { get; set; } = "";

        [Required]
        public string contrasena { get; set; } = "";

        [Required]
        public bool DebeCambiarPass { get; set; } = false;

    }
    public class LoginResponseDto
    {

        [Required]
        public int NoUsuario { get; set; }


        [Required]
        public string Token { get; set; } = null!;


        [Required]
        public bool EsManager { get; set; } = false;

        [Required]
        public bool DebeCambiarPass { get; set; }
    }
}
