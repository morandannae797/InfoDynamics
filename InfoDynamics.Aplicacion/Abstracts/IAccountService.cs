using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Aplicacion.Abstracts
{
    public interface IAccountService
    {
        Task RefreshtokenAsync(string? refreshToken);
        Task<LoginResponseDto> LoginAsync(loginDto loginDto);
        //Task<Usuario> LoginAsync(loginDto loginDto);
    }
}
