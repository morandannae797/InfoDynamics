using InfoDynamics.Aplicacion.dtos;
using System;
using System.Collections.Generic;
using System.Text;
using InfoDynamics.Aplicacion.servicios.Servicios;

namespace InfoDynamics.Aplicacion.servicios
{
    public interface IHmacServicio
    {
        bool VerifySignature(loginDto request, string providedsignature);
    }
}
