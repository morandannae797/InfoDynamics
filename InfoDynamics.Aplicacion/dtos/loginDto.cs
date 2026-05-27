using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InfoDynamics.Aplicacion.dtos
{
    public class loginDto
    {
        public string identificador { get; set; } = "";
        public string contrasena { get; set; } = "";
       
    }
    public class LoginResponseDto
    {
        public int NoUsuario { get; set; }

        public string Token { get; set; } = null!;

        [Column("es_admin")]
        public bool EsManager { get; set; } = false;
    }
}
