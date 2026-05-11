using InfoDynamics.Aplicacion.dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Aplicacion.Abstracts
{
    public interface IAccountService
    {
        Task RefreshtokenAsync(string? refreshToken);
        Task LoginAsync(loginDto loginDto);
    }
}
