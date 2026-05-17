using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class VacacionWriteService :
        IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VacacionWriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CREAR VACACION
        public async Task AddAsync(VacacionDto.VacacionCreateDto dto)
        {
            // VALIDAR FECHAS
            if (dto.FechaFin < dto.FechaInicio)
            {
                throw new Exception("La fecha fin no puede ser menor a la fecha inicio.");
            }

            // OBTENER PERIODOS
            var periodos = await _unitOfWork.Repository<Periodo>()
                .GetAllAsync();

            // BUSCAR PERIODO ABIERTO
            var periodo = periodos.FirstOrDefault(p =>
                p.estado == "Abierto" &&
                dto.FechaInicio.Date >= p.fecha_inicio.ToDateTime(TimeOnly.MinValue) &&
                dto.FechaFin.Date <= p.fecha_fin.ToDateTime(TimeOnly.MinValue)
            );

            // VALIDAR PERIODO
            if (periodo == null)
            {
                throw new Exception("No existe un periodo abierto para esas fechas.");
            }

            // CREAR VACACION
            var vacacion = new Vacacion
            {
                fecha_solicito = DateTime.Now,

                fecha_inicio = DateOnly.FromDateTime(dto.FechaInicio),

                fecha_fin = DateOnly.FromDateTime(dto.FechaFin),

                estado = "Pendiente",

                no_usuario = dto.SolicitanteId
            };

            // GUARDAR
            await _unitOfWork.Repository<Vacacion>()
                .AddAsync(vacacion);

            await _unitOfWork.SaveChangesAsync();
        }

        // APROBAR O RECHAZAR
        public async Task UpdateAsync(VacacionDto.VacacionAprobacionDto dto)
        {
            var vacacion = await _unitOfWork.Repository<Vacacion>()
                .GetByIdAsync(dto.VacacionId);

            // VALIDAR EXISTENCIA
            if (vacacion == null)
            {
                throw new Exception("Vacación no encontrada.");
            }

            // VALIDAR ESTADO
            if (vacacion.estado != "Pendiente")
            {
                throw new Exception("La solicitud ya fue evaluada.");
            }

            // ACTUALIZAR DATOS
            vacacion.estado = dto.EstadoDecision;

            vacacion.id_administrador = dto.AprobadorId;

            vacacion.fecha_aprobo = DateTime.Now;

            // GUARDAR CAMBIOS
            await _unitOfWork.Repository<Vacacion>()
                .UpdateAsync(vacacion);

            await _unitOfWork.SaveChangesAsync();
        }
// METODO OBLIGATORIO POR LA INTERFAZ
public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

