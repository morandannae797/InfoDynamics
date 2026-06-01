using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class NotificacionHrsServicio
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public NotificacionHrsServicio(
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<int> EnviarRecordatorioHorasAsync()
        {
            var ahora = DateTime.Now;
            var hoy = ahora.Date;

            var periodos = await _unitOfWork.Repository<Periodo>().GetAllAsync();

            var periodoActual = periodos
                .Where(p =>
                    p.fecha_inicio.Date <= hoy &&
                    p.fecha_fin.Date >= hoy &&
                    (p.estado == "En proceso" || p.estado == "Abierto"))
                .OrderByDescending(p => p.fecha_fin)
                .FirstOrDefault();

            if (periodoActual == null)
                return 0;

            var cierreCaptura = periodoActual.fecha_fin.Date
                .AddDays(1)
                .AddTicks(-1);

            var tiempoRestante = cierreCaptura - ahora;

            // Solo enviar si  24 horas antes del cierre.

            if (tiempoRestante.TotalHours < 23 || tiempoRestante.TotalHours > 25)
                return 0;

            var usuarios = await _unitOfWork.Repository<Usuario>().GetAllAsync();
            var registros = await _unitOfWork.Repository<Registro>().GetAllAsync();

            var empleadosActivos = usuarios
                .Where(u =>
                    u.estado_cuenta == true &&
                    u.es_manager == false &&
                    !string.IsNullOrWhiteSpace(u.email))
                .ToList();

            var empleadosSinRegistro = empleadosActivos
                .Where(u => !registros.Any(r =>
                    r.no_usuario == u.no_usuario &&
                    r.id_periodo == periodoActual.id_periodo))
                .ToList();

            foreach (var empleado in empleadosSinRegistro)
            {
                var nombreCompleto = $"{empleado.nombre} {empleado.ap_paterno}".Trim();

                var asunto = "Recordatorio: registra tus horas";

                var html = $"""
                <!DOCTYPE html>
                <html lang="es">
                <head>
                  <meta charset="UTF-8"/>
                  <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
                </head>
                <body style="margin:0;padding:40px 0;background-color:#f4f4f4;font-family:Arial,sans-serif;">

                  <div style="max-width:520px;margin:0 auto;background:#ffffff;border-radius:8px;padding:36px;">

                    <h1 style="margin:0 0 24px;font-size:24px;font-weight:700;color:#1a1a1a;text-align:center;">
                      InfoDynamics
                    </h1>

                    <p style="margin:0 0 20px;font-size:15px;color:#1a1a1a;">
                      Hola <strong>{nombreCompleto}</strong>,
                    </p>

                    <p style="margin:0 0 20px;font-size:15px;color:#1a1a1a;">
                      Te recordamos que aún no has registrado tus horas del periodo actual.
                    </p>

                    <p style="margin:0 0 8px;font-size:15px;color:#1a1a1a;">
                      <strong>Periodo:</strong>
                    </p>

                    <p style="margin:0 0 24px;font-size:15px;color:#1a1a1a;">
                      {periodoActual.fecha_inicio:dd/MM/yyyy} - {periodoActual.fecha_fin:dd/MM/yyyy}
                    </p>

                    <p style="margin:0 0 24px;font-size:15px;color:#1a1a1a;">
                      El periodo cerrará en aproximadamente 24 horas. Por favor registra tus horas antes del cierre.
                    </p>

                    <p style="margin:0;font-size:12px;color:#999999;text-align:center;">
                      Este es un mensaje automático. No respondas a este correo.
                    </p>

                  </div>

                </body>
                </html>
                """;

                await _emailService.SendEmailAsync(
                    empleado.email,
                    asunto,
                    html);
            }

            return empleadosSinRegistro.Count;
        }
    }
}