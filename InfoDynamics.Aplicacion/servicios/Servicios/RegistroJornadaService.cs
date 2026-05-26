using AutoMapper;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using System;
using System.Threading.Tasks;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class RegistroJornadaService
        : WriteServiceAsync<Registro, RegistroCreateDto, RegistroUpdateDto>
    {

        public RegistroJornadaService(
            IUnitOfWork unitOfWork,
            IMapper mapper
        ) : base(unitOfWork, mapper)
        {
        }

        public override async Task AddAsync(RegistroCreateDto dto)
        {

            if (dto.Horas < 0)
            {
                throw new BadRequestException(
                    "Las horas no pueden ser negativas."
                );
            }



            var proyecto = await _unitOfWork
                .Repository<Proyecto>()
                .GetAsync(p => p.codigo == dto.Codigo);



            if (proyecto == null)
            {
                throw new EntityNotFoundException(
                    "El proyecto no existe."
                );
            }


            if (proyecto.id_empresa <= 0)
            {
                throw new BadRequestException(
                    "El proyecto no tiene empresa asignada."
                );
            }


            if (
                !proyecto.codigo.StartsWith(
                    "L",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                !proyecto.codigo.StartsWith(
                    "M",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "El código del proyecto no tiene clasificación válida."
                );
            }


            await base.AddAsync(dto);
        }





        public override async Task UpdateAsync(RegistroUpdateDto dto)
        {


            if (dto.Horas < 0)
            {
                throw new BadRequestException(
                    "Las horas no pueden ser negativas."
                );
            }


            var proyecto = await _unitOfWork
                .Repository<Proyecto>()
                .GetAsync(p => p.codigo == dto.Codigo);

            if (proyecto == null)
            {
                throw new EntityNotFoundException(
                    "El proyecto no existe."
                );
            }



            if (proyecto.id_empresa <= 0)
            {
                throw new BadRequestException(
                    "El proyecto no tiene empresa asignada."
                );
            }


            if (
                !proyecto.codigo.StartsWith(
                    "L",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                !proyecto.codigo.StartsWith(
                    "M",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "El código del proyecto no tiene clasificación válida."
                );
            }


            await base.UpdateAsync(dto);
        }
    }
    //ACOMODALO AQUI PORFA QUE SE ME HACE QUE
    //ESTA MUY GRANDE LA CLASE, SEPARA LAS RESPONSABILIDADES EN OTRAS CLASES PARA QUE SEA MAS FACIL DE MANTENER Y ENTENDER, GRACIAS


    public class JornadaValidacionService
    {
       
        // Validar periodo 
        public void ValidarPeriodo(Periodo periodo)
        {
            if (periodo.estado != "Abierto")
                throw new BadRequestException("Solo se pueden registrar horas en un periodo con estado Abierto.");
        }

    }

    public class JornadaRegistroService
    {
        // Registrar jornadas
        private readonly IUnitOfWork _unitOfWork;
        private readonly JornadaValidacionService _validacion;

        public JornadaRegistroService(IUnitOfWork unitOfWork, JornadaValidacionService validacion)
        {
            _unitOfWork = unitOfWork;
            _validacion = validacion;
        }

        // S5.3.1.6 
        // S5.3.1.10 
        // S5.5.1
        public async Task RegistrarAsync(RegistroCreateDto dto)
        {
            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(dto.PeriodoId);

            if (periodo == null)
                throw new EntityNotFoundException($"El periodo con ID {dto.PeriodoId} no existe.");

            _validacion.ValidarPeriodo(periodo);

            var duplicado = await _unitOfWork.Repository<Registro>()
                .FirstOrDefaultAsync(
                    r => r.no_usuario == dto.NoUsuario
                      && r.fecha.Date == dto.Fecha.Date
                      && r.codigo == dto.Codigo
                );

            if (duplicado != null)
                throw new ConflictException("Ya existe un registro para ese usuario en ese dia y codigo.");

            var registro = new Registro
            {
                fecha = dto.Fecha,
                horas = dto.Horas,
                no_usuario = dto.NoUsuario,
                id_periodo = dto.PeriodoId,
                codigo = dto.Codigo
            };

            await _unitOfWork.Repository<Registro>().AddAsync(registro);
            await _unitOfWork.SaveChangesAsync(); // S5.5.1
        }

        // S5.6 / S5.6.1
        public async Task EditarAsync(RegistroUpdateDto dto)
        {
            var registro = await _unitOfWork.Repository<Registro>().GetByIdAsync(dto.RegistroId);

            if (registro == null)
                throw new EntityNotFoundException("Registro no encontrado.");

            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(dto.PeriodoId);

            if (periodo == null)
                throw new EntityNotFoundException($"El periodo con ID {dto.PeriodoId} no existe.");

            _validacion.ValidarPeriodo(periodo);

            registro.fecha = dto.Fecha;
            registro.horas = dto.Horas;
            registro.id_periodo = dto.PeriodoId;
            registro.codigo = dto.Codigo;

            await _unitOfWork.Repository<Registro>().UpdateAsync(registro);
            await _unitOfWork.SaveChangesAsync(); // S5.5.1
        }

    }

    public class JornadaCalculoService
    {
        private readonly IUnitOfWork _unitOfWork;

    public JornadaCalculoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // S5.4
    public async Task<decimal> GetHorasSemanaAsync(int noUsuario, DateTime fecha)
    {
        var inicioSemana = fecha.Date.AddDays(-(int)fecha.DayOfWeek + 1);
        var finSemana = inicioSemana.AddDays(6);

        var registros = await _unitOfWork.Repository<Registro>().GetAllAsync();

        return registros
            .Where(r => r.no_usuario == noUsuario
                     && r.fecha.Date >= inicioSemana
                     && r.fecha.Date <= finSemana)
            .Sum(r => r.horas);
    }

    }
}