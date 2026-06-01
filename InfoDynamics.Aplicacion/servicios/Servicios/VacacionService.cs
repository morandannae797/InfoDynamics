using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InfoDynamics.Aplicacion.servicios
{
    public class VacacionAprobacionService : IVacacionAprobacionService
    {
        private readonly IGenericRepository<Vacacion> _vacacionRepository;
        private readonly IGenericRepository<Usuario_manager> _usuarioManagerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService; // Inyectamos el servicio de correo

        public VacacionAprobacionService(
            IGenericRepository<Vacacion> vacacionRepository,
            IGenericRepository<Usuario_manager> usuarioManagerRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService) // Agregado al constructor
        {
            _vacacionRepository = vacacionRepository;
            _usuarioManagerRepository = usuarioManagerRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task EvaluarVacacionAsync(int idVacacion, VacacionDto.VacacionAprobacionDto dto, int noUsuarioManager)
        {
            var vacacion = await _vacacionRepository.GetByIdAsync(idVacacion);

            if (vacacion == null)
                throw new EntityNotFoundException("La solicitud de vacaciones no existe.");

            if (vacacion.estado != "Pendiente")
                throw new ConflictException("La solicitud ya fue evaluada anteriormente.");

            var relaciones = await _usuarioManagerRepository.GetAllAsync();
            bool pertenece = relaciones.Any(x => x.no_usuario_manager == noUsuarioManager && x.no_usuario == vacacion.no_usuario);

            if (!pertenece)
                throw new UnauthorizedException("No tienes permiso para aprobar solicitudes de este usuario.");

            vacacion.estado = dto.EstadoDecision;

            if (vacacion.estado == "Aprobada")
            {
                var periodoActivo = await _unitOfWork.Repository<Periodo>().GetAsync(p => p.estado == "Abierto");

                if (periodoActivo == null)
                    throw new ConflictException("No hay periodos abiertos para registrar las vacaciones.");

                var nuevoRegistro = new Registro
                {
                    no_usuario = vacacion.no_usuario,
                    fecha = vacacion.fecha_inicio.ToDateTime(TimeOnly.MinValue),
                    horas = 8,
                    id_periodo = periodoActivo.id_periodo
                };

                await _unitOfWork.Repository<Registro>().AddAsync(nuevoRegistro);
            }

            await _unitOfWork.SaveChangesAsync();

            // --- Lógica de notificación al empleado ---
            try
            {
                var empleado = await _unitOfWork.Repository<Usuario>().GetByIdAsync(vacacion.no_usuario);
                if (empleado != null)
                {
                    string asunto = $"Actualización de solicitud de vacaciones";
                    string cuerpo = $"Hola {empleado.nombre}, tu solicitud de vacaciones ha sido actualizada a: <strong>{vacacion.estado}</strong>.";

                    // Disparo asíncrono para no bloquear la respuesta de la API
                    _ = _emailService.SendEmailAsync(empleado.email, asunto, cuerpo);
                }
            }
            catch (Exception ex)
            {
                // Silenciamos el error para no afectar la transacción de la BD
                Console.WriteLine($"Error al enviar correo de notificación: {ex.Message}");
            }
        }
    }
}