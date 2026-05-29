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
     
            var vacacion = await _vacacionRepository.GetByIdAsync(idVacacion);

            if (vacacion == null)
                throw new EntityNotFoundException("La solicitud de vacaciones no existe.");

            if (vacacion.estado != "Pendiente")
                throw new ConflictException("La solicitud ya fue evaluada anteriormente.");


            var relaciones = await _usuarioManagerRepository.GetAllAsync();
            bool pertenece = relaciones.Any(x => x.no_usuario_manager == noUsuarioManager && x.no_usuario == vacacion.no_usuario);

            if (!pertenece)
                throw new UnauthorizedException("No puedes modificar solicitudes de otro equipo.");

            vacacion.estado = dto.EstadoDecision;

            if (vacacion.estado == "Aprobada")
            {
        
                var periodoActivo = await _unitOfWork.Repository<Periodo>()
                    .GetAsync(p => p.estado == "Abierto");

                var proyectoActivo = await _unitOfWork.Repository<Proyecto>()
                    .GetAsync(p => true);

                if (periodoActivo == null)
                    throw new ConflictException("No hay periodos abiertos para registrar las vacaciones.");

                if (proyectoActivo == null)
                    throw new ConflictException("No hay proyectos configurados en el sistema.");

                var nuevoRegistro = new Registro
                {
                    no_usuario = vacacion.no_usuario,
                    fecha = vacacion.fecha_inicio.ToDateTime(TimeOnly.MinValue),
                    horas = 8,
                    codigo = "VAC",
                    id_periodo = periodoActivo.id_periodo,
                
                };

                await _unitOfWork.Repository<Registro>().AddAsync(nuevoRegistro);
            }
            else if (vacacion.estado == "Rechazada")
            {
            
            }
            else
            {
                throw new ConflictException("Estado de decisión no válido. Debe ser 'Aprobada' o 'Rechazada'.");
            }

  
            await _unitOfWork.SaveChangesAsync();
        }
    }
}