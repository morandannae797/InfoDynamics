using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class PeriodoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PeriodoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PeriodoDto> GenerarPeriodoAsync()
        {
            var periodos = await _unitOfWork.Repository<Periodo>().GetAllAsync();

            var ultimoPeriodo = periodos.OrderByDescending(p => p.fecha_fin).FirstOrDefault();

            if (ultimoPeriodo != null)
            {
                DateTime finCiclo = ultimoPeriodo.fecha_fin.Date.AddDays(7).AddTicks(-1);

                if (DateTime.Now <= finCiclo)
                    throw new BadRequestException("No se puede generar un nuevo periodo porque el ciclo actual todavía no ha terminado.");

                ultimoPeriodo.estado = "Cerrado";

                await _unitOfWork.Repository<Periodo>().UpdateAsync(ultimoPeriodo);
            }

            var fechaInicio = ultimoPeriodo == null ? DateTime.Today : ultimoPeriodo.fecha_fin.Date.AddDays(7);

            var fechaFin = fechaInicio.AddDays(13);

            var nuevoPeriodo = new Periodo
            {
                fecha_inicio = fechaInicio,
                fecha_fin = fechaFin,
                estado = "Abierto"
            };

            await _unitOfWork.Repository<Periodo>().AddAsync(nuevoPeriodo);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PeriodoDto>(nuevoPeriodo);
        }
        public async Task CerrarPeriodoAsync(int idPeriodo)
        {
            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(idPeriodo);

            if (periodo == null) throw new EntityNotFoundException("El periodo no existe.");

            periodo.estado = "Cerrado";

            await _unitOfWork.Repository<Periodo>().UpdateAsync(periodo);

            await _unitOfWork.SaveChangesAsync();
        }
        public void ValidarAccesoPeriodo(Periodo periodo, string rolUsuario, string accion)
        {
            DateTime ahora = DateTime.Now;

            DateTime finCaptura = periodo.fecha_fin.Date.AddDays(1).AddTicks(-1);

            DateTime diaBloqueoInicio = periodo.fecha_fin.Date.AddDays(1);
            DateTime diaBloqueoFin = periodo.fecha_fin.Date.AddDays(2).AddTicks(-1);

            DateTime gestionManagerInicio = periodo.fecha_fin.Date.AddDays(2);
            DateTime gestionManagerFin = periodo.fecha_fin.Date.AddDays(7).AddTicks(-1);

            if (ahora <= finCaptura)
            {
                if (rolUsuario != "Empleado")
                    throw new UnauthorizedException("Solo los empleados pueden registrar o modificar horas durante el periodo abierto.");

                return;
            }

            if (ahora >= diaBloqueoInicio && ahora <= diaBloqueoFin)
            {
                throw new BadRequestException("El periodo está en cierre inicial. Nadie puede modificar registros durante este día.");
            }

            if (ahora >= gestionManagerInicio && ahora <= gestionManagerFin)
            {
                if (rolUsuario == "Empleado" && accion != "Consultar")
                    throw new UnauthorizedException("Los empleados solo pueden consultar durante la semana de gestión.");

                if (rolUsuario == "Manager")
                    return;

                throw new UnauthorizedException("Solo los managers pueden gestionar registros durante esta semana.");
            }

            throw new BadRequestException("El ciclo del periodo ya finalizó. Debe generarse un nuevo periodo.");
        }
    }
}