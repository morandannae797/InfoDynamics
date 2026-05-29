using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;

namespace InfoDynamics.Aplicacion.servicios
{
    public class VacacionAprobacionService : IVacacionAprobacionService
    {
        private readonly IGenericRepository<Vacacion> _vacacionRepository;
        private readonly IGenericRepository<Usuario_manager> _usuarioManagerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VacacionAprobacionService(
            IGenericRepository<Vacacion> vacacionRepository,
            IGenericRepository<Usuario_manager> usuarioManagerRepository,
            IUnitOfWork unitOfWork)
        {
            _vacacionRepository = vacacionRepository;
            _usuarioManagerRepository = usuarioManagerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task EvaluarVacacionAsync(int idVacacion, VacacionDto.VacacionAprobacionDto dto, int noUsuarioManager)
        {
            // 1. Obtener la solicitud
            var vacacion = await _vacacionRepository.GetByIdAsync(idVacacion);

            if (vacacion == null)
                throw new EntityNotFoundException("La solicitud de vacaciones no existe.");

            if (vacacion.estado != "Pendiente")
                throw new ConflictException("La solicitud ya fue evaluada anteriormente.");

            // 2. Validar pertenencia al equipo
            var relaciones = await _usuarioManagerRepository.GetAllAsync();
            bool pertenece = relaciones.Any(x => x.no_usuario_manager == noUsuarioManager && x.no_usuario == vacacion.no_usuario);

            if (!pertenece)
                throw new UnauthorizedException("No puedes modificar solicitudes de otro equipo.");

            // 3. Procesar decisión
            vacacion.estado = dto.EstadoDecision;

            // 4. Lógica de Aprobación vs Rechazo
            if (vacacion.estado == "Aprobada")
            {
                // BÚSQUEDA DINÁMICA: Periodo activo y proyecto
                var periodoActivo = await _unitOfWork.Repository<Periodo>()
                    .GetAsync(p => p.estado == "Abierto");

                var proyectoActivo = await _unitOfWork.Repository<Proyecto>()
                    .GetAsync(p => true);

                if (periodoActivo == null)
                    throw new ConflictException("No hay periodos abiertos para registrar las vacaciones.");

                if (proyectoActivo == null)
                    throw new ConflictException("No hay proyectos configurados en el sistema.");

                // Creamos el registro de jornada
                var nuevoRegistro = new Registro
                {
                    no_usuario = vacacion.no_usuario,
                    fecha = vacacion.fecha_inicio.ToDateTime(TimeOnly.MinValue),
                    horas = 8,
                    codigo = "VAC",
                    id_periodo = periodoActivo.id_periodo,
                    id_proyecto = proyectoActivo.codigo // Usamos el código al ser el identificador
                };

                await _unitOfWork.Repository<Registro>().AddAsync(nuevoRegistro);
            }
            else if (vacacion.estado == "Rechazada")
            {
                // Si es rechazada, no se hace registro de horas, solo se cambia el estado.
            }
            else
            {
                throw new ConflictException("Estado de decisión no válido. Debe ser 'Aprobada' o 'Rechazada'.");
            }

            // 5. Guardar todo en una transacción atómica
            await _unitOfWork.SaveChangesAsync();
        }
    }
}