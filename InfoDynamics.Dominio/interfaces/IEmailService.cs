using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Dominio.interfaces
{
    public interface IEmailService
    {
        Task EnviarNotificacionVacacionesAsync(string correoDestino, string nombreEmpleado, string fechaInicio);

        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendTemporaryPasswordAsync(
           string correoDestino,
           string nombreEmpleado,
           string temporaryPassword,
           string loginUrl);
        Task EnvioResetContra(
string correoDestino,
string nombreEmpleado,
string digitoreseteo
);
    }
}
