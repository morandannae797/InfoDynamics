using System;
using System.Collections.Generic;
using System.Text;
using InfoDynamics.Dominio.Entidades;

namespace InfoDynamics.Aplicacion.Abstracts
{
    public interface IAuthTokenProcessor      {
        (string token, DateTime expiresUTC) GenerateJwtToken(Usuario user);
        string GenerateRefreshToken();
        void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);

    }
}
