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

            // Validamos que solo se puedan procesar solicitudes pendientes
            if (vacacion.estado != "Pendiente")
                throw new ConflictException("La solicitud ya fue evaluada anteriormente.");

            // 2. Validar que el manager tiene autoridad sobre el empleado
            var relaciones = await _usuarioManagerRepository.GetAllAsync();
            bool pertenece = relaciones.Any(x => x.no_usuario_manager == noUsuarioManager && x.no_usuario == vacacion.no_usuario);

            if (!pertenece)
                throw new UnauthorizedException("No tienes permiso para aprobar solicitudes de este usuario.");

            // 3. Aplicar la decisión del manager
            vacacion.estado = dto.EstadoDecision;

            // 4. Si se aprueba, realizamos el registro de horas
            if (vacacion.estado == "Aprobada")
            {
                var periodoActivo = await _unitOfWork.Repository<Periodo>().GetAsync(p => p.estado == "Abierto");
                var proyectoActivo = await _unitOfWork.Repository<Proyecto>().GetAsync(p => true);

                if (periodoActivo == null)
                    throw new ConflictException("No hay periodos abiertos para registrar las vacaciones.");

                if (proyectoActivo == null)
                    throw new ConflictException("No hay proyectos configurados.");

                
                // 1. Verificamos si el código ingresado existe realmente en la tabla Proyecto
                var proyectoExiste = await _unitOfWork.Repository<Proyecto>().GetAsync(p => p.codigo == dto.CodigoProyecto);

                if (proyectoExiste == null)
                {
                    // Esto es mucho más amigable que el error de SQL
                    throw new ConflictException($"El código de proyecto '{dto.CodigoProyecto}' no es válido.");
                }

                // 2. Si es válido, procedemos con el registro
                var nuevoRegistro = new Registro
                {
                    no_usuario = vacacion.no_usuario,
                    fecha = vacacion.fecha_inicio.ToDateTime(TimeOnly.MinValue),
                    horas = 8,
                    codigo = dto.CodigoProyecto, // El que capturó el usuario
                    id_periodo = periodoActivo.id_periodo
                };

                await _unitOfWork.Repository<Registro>().AddAsync(nuevoRegistro);
            }

            // 5. Guardar todo en una transacción atómica
            await _unitOfWork.SaveChangesAsync();
        }
    }
}