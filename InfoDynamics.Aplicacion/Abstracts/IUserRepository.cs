using InfoDynamics.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Aplicacion.Abstracts
{
    public interface IUserRepository
    {
        Task<Usuario?> GetUserbyRefreshToken(string refreshToken);
        Task SaveRefreshTokenAsync(int noUsuario, string refreshToken, DateTime expires);
    }
}
