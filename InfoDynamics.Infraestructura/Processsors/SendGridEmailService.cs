
using InfoDynamics.Dominio.interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InfoDynamics.Infraestructura.Processors
{
    public class SendSmtpEmailService : IEmailService, IDisposable
    {
        private readonly SmtpOptions _options;
        private readonly SmtpClient _client;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public SendSmtpEmailService(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
            _client = new SmtpClient();
        }

        private async Task EnsureConnectedAsync()
        {
            if (_client.IsConnected && _client.IsAuthenticated)
                return;

            if (_client.IsConnected)
                await _client.DisconnectAsync(true);

            await _client.ConnectAsync(
                _options.Host,
                _options.Port,
                SecureSocketOptions.StartTls);

            await _client.AuthenticateAsync(
                _options.Username,
                _options.Password);
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email is required.", nameof(to));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            await _lock.WaitAsync();
            try
            {
                await EnsureConnectedAsync();
                await _client.SendAsync(message);
            }
            catch
            {
                try { await _client.DisconnectAsync(false); } catch { }
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task EnviarNotificacionVacacionesAsync(
            string correoDestino,
            string nombreEmpleado,
            string fechaInicio)
        {
            var asunto = "Aprobación de Solicitud de Vacaciones";
            var html = $"""
                <h3>Hola {nombreEmpleado},</h3>
                <p>Tus vacaciones a partir del <strong>{fechaInicio}</strong>
                han sido aprobadas.</p>
                """;

            await SendEmailAsync(correoDestino, asunto, html);
        }

        public void Dispose()
        {
            if (_client.IsConnected)
                _client.Disconnect(true);

            _client.Dispose();
            _lock.Dispose();
        }
        public async Task SendTemporaryPasswordAsync(
    string correoDestino,
    string nombreEmpleado,
    string temporaryPassword,
    string loginUrl)
        {
            var asunto = "Acceso a InfoDynamics";

            var html = $"""
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8"/>
      <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    </head>
    <body style="margin:0;padding:40px 0;background-color:#9b59b6;font-family:Arial,sans-serif;">

      <div style="max-width:480px;margin:0 auto;background:#ffffff;border-radius:8px;padding:40px;">

        <h1 style="margin:0 0 24px;font-size:24px;font-weight:700;color:#1a1a1a;text-align:center;">
          InfoDynamics
        </h1>

        <p style="margin:0 0 20px;font-size:15px;color:#1a1a1a;">
          Hola <strong>{nombreEmpleado}</strong>, tu cuenta en InfoDynamics ha sido creada.
        </p>

        <p style="margin:0 0 4px;font-size:13px;color:#9b59b6;">Correo electrónico</p>
        <p style="margin:0 0 16px;font-size:15px;color:#1a1a1a;">{correoDestino}</p>

        <p style="margin:0 0 4px;font-size:13px;color:#9b59b6;">Contraseña temporal</p>
        <p style="margin:0 0 24px;font-size:15px;color:#1a1a1a;">{temporaryPassword}</p>

        <p style="margin:0 0 28px;font-size:15px;color:#1a1a1a;">
          El siguiente enlace te redirigirá a la página de inicio de sesión:
          <a href="{loginUrl}" style="color:#9b59b6;text-decoration:none;font-weight:600;">{loginUrl}</a>
        </p>

        <p style="margin:0;font-size:12px;color:#aaaaaa;text-align:center;">
          Este es un mensaje automático. No respondas a este correo.
        </p>

      </div>
    </body>
    </html>
    """;

            await SendEmailAsync(correoDestino, asunto, html);
        }

        public async Task EnvioResetContra(
       string correoDestino,
       string nombreEmpleado,
       string digitoreseteo
       )
        {
            var asunto = "Reseteo de contraseña para InfoDynamics";

            var html = $"""
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="UTF-8"/>
      <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    </head>
    <body style="margin:0;padding:40px 0;background-color:#9b59b6;font-family:Arial,sans-serif;">

      <div style="max-width:480px;margin:0 auto;background:#ffffff;border-radius:8px;padding:40px;">

        <h1 style="margin:0 0 24px;font-size:24px;font-weight:700;color:#1a1a1a;text-align:center;">
          InfoDynamics
        </h1>

        <p style="margin:0 0 20px;font-size:15px;color:#1a1a1a;">
          Hola se ha solicitado el cambio de .
        </p>

        <p style="margin:0 0 4px;font-size:13px;color:#9b59b6;">Correo electrónico</p>
        <p style="margin:0 0 16px;font-size:15px;color:#1a1a1a;">{correoDestino}</p>

        <p style="margin:0 0 4px;font-size:13px;color:#9b59b6;">Codigo de verificacion</p>
        <p style="margin:0 0 24px;font-size:15px;color:#1a1a1a;">{digitoreseteo}</p>

        

        <p style="margin:0;font-size:12px;color:#aaaaaa;text-align:center;">
          Este es un mensaje automático. No respondas a este correo.
        </p>

      </div>
    </body>
    </html>
    """;

            await SendEmailAsync(correoDestino, asunto, html);
        }
    }
}