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

        public async Task<PeriodoResponseDto> GenerarPeriodoAsync()
        {
            var periodos = await _unitOfWork.Repository<Periodo>().GetAllAsync();

            var ultimoPeriodo = periodos.OrderByDescending(p => p.fecha_fin).FirstOrDefault();

            if (ultimoPeriodo != null && ultimoPeriodo.estado == "Abierto")
            {
                ultimoPeriodo.estado = "Cerrado";
                await _unitOfWork.Repository<Periodo>().UpdateAsync(ultimoPeriodo);
            }

            var fechaInicio = ultimoPeriodo == null ? DateTime.Today : ultimoPeriodo.fecha_fin.Date.AddDays(1);

            var fechaFin = fechaInicio.AddDays(13);

            var nuevoPeriodo = new Periodo { fecha_inicio = fechaInicio, fecha_fin = fechaFin, estado = "Abierto" };

            await _unitOfWork.Repository<Periodo>().AddAsync(nuevoPeriodo);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PeriodoResponseDto>(nuevoPeriodo);
        }

        public async Task CerrarPeriodoAsync(int idPeriodo)
        {
            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(idPeriodo);

            if (periodo == null) throw new EntityNotFoundException("El periodo no existe.");

            periodo.estado = "Cerrado";

            await _unitOfWork.Repository<Periodo>().UpdateAsync(periodo);

            await _unitOfWork.SaveChangesAsync();
        }

        public void ValidarAccesoPeriodo(Periodo periodo, string rolUsuario)
        {
            if (periodo.estado == "Cerrado") throw new BadRequestException("El periodo está cerrado y no permite modificaciones.");

            if (periodo.estado == "Abierto" && rolUsuario != "Empleado") throw new UnauthorizedException("Solo los empleados pueden registrar o modificar horas en un periodo abierto.");
        }
    }
}