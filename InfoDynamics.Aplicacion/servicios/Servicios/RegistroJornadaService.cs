using AutoMapper;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class RegistroJornadaService : WriteServiceAsync<Registro, RegistroCreateDto, RegistroUpdateDto>
    {
        private readonly JornadaValidacionService _validacion;

        public RegistroJornadaService(IUnitOfWork unitOfWork, IMapper mapper, JornadaValidacionService validacion) : base(unitOfWork, mapper)
        {
            _validacion = validacion;
        }

        public override async Task AddAsync(RegistroCreateDto dto)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            await base.AddAsync(dto);
        }

        public override async Task UpdateAsync(RegistroUpdateDto dto)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            await base.UpdateAsync(dto);
        }
    }


    public class JornadaValidacionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JornadaValidacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void ValidarHoras(decimal horas)
        {
            if (horas < 0) throw new BadRequestException("Las horas no pueden ser negativas.");
        }

        public void ValidarPeriodo(Periodo periodo)
        {
            if (periodo.estado != "Abierto") throw new BadRequestException("Solo se pueden registrar horas en un periodo con estado Abierto.");
        }

        public async Task ValidarProyectoAsync(string codigo)
        {
            var proyecto = await _unitOfWork.Repository<Proyecto>().GetAsync(p => p.codigo == codigo);

            if (proyecto == null) throw new EntityNotFoundException("El proyecto no existe.");

            if (proyecto.id_empresa <= 0) throw new BadRequestException("El proyecto no tiene empresa asignada.");

            if (!proyecto.codigo.StartsWith("L", StringComparison.OrdinalIgnoreCase) && !proyecto.codigo.StartsWith("M", StringComparison.OrdinalIgnoreCase)) throw new BadRequestException("El código del proyecto no tiene clasificación válida.");
        }
    }

    public class JornadaRegistroService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JornadaValidacionService _validacion;

        public JornadaRegistroService(IUnitOfWork unitOfWork, JornadaValidacionService validacion)
        {
            _unitOfWork = unitOfWork;
            _validacion = validacion;
        }

        public async Task RegistrarAsync(RegistroCreateDto dto)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(dto.PeriodoId);

            if (periodo == null) throw new EntityNotFoundException($"El periodo con ID {dto.PeriodoId} no existe.");

            _validacion.ValidarPeriodo(periodo);

            var duplicado = await _unitOfWork.Repository<Registro>().FirstOrDefaultAsync(r => r.no_usuario == dto.NoUsuario && r.fecha.Date == dto.Fecha.Date && r.codigo == dto.Codigo);

            if (duplicado != null) throw new ConflictException("Ya existe un registro para ese usuario en ese día y código.");

            var registro = new Registro { fecha = dto.Fecha, horas = dto.Horas, no_usuario = dto.NoUsuario, id_periodo = dto.PeriodoId, codigo = dto.Codigo };

            await _unitOfWork.Repository<Registro>().AddAsync(registro);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditarAsync(RegistroUpdateDto dto)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            var registro = await _unitOfWork.Repository<Registro>().GetByIdAsync(dto.RegistroId);

            if (registro == null) throw new EntityNotFoundException("Registro no encontrado.");

            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(dto.PeriodoId);

            if (periodo == null) throw new EntityNotFoundException($"El periodo con ID {dto.PeriodoId} no existe.");

            _validacion.ValidarPeriodo(periodo);

            registro.fecha = dto.Fecha;

            registro.horas = dto.Horas;

            registro.id_periodo = dto.PeriodoId;

            registro.codigo = dto.Codigo;

            await _unitOfWork.Repository<Registro>().UpdateAsync(registro);

            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class JornadaCalculoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JornadaCalculoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> GetHorasSemanaAsync(int noUsuario, DateTime fecha)
        {
            var inicioSemana = fecha.Date.AddDays(-(int)fecha.DayOfWeek + 1);

            var finSemana = inicioSemana.AddDays(6);

            var registros = await _unitOfWork.Repository<Registro>().GetAllAsync();

            return registros.Where(r => r.no_usuario == noUsuario && r.fecha.Date >= inicioSemana && r.fecha.Date <= finSemana).Sum(r => r.horas);
        }

        public void ValidarPeriodoEmpleado(Periodo periodo, string rolUsuario)
        {
            if (periodo.estado == "Cerrado") throw new BadRequestException("El periodo está cerrado y no permite modificaciones.");

            if (rolUsuario != "Empleado") throw new UnauthorizedException("Solo los empleados pueden registrar o modificar horas.");
        }
    }


    }