using AutoMapper;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class RegistroJornadaService
       : WriteServiceAsync<Registro, RegistroCreateDto, RegistroDto>
    {
        private readonly JornadaValidacionService _validacion;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public RegistroJornadaService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            JornadaValidacionService validacion,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, mapper)
        {
            _validacion = validacion;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public override async Task AddAsync(RegistroCreateDto dto)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            await base.AddAsync(dto);
        }




        public override async Task UpdateAsync(RegistroDto dto)
        {
            _validacion.ValidarHoras(dto.Horas);
            await _validacion.ValidarProyectoAsync(dto.Codigo);

            //
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(dto.NoUsuario);
            if (usuario == null)
                throw new EntityNotFoundException($"El usuario {dto.NoUsuario} no existe.");



            var repo = _unitOfWork.Repository<Registro>();

            

            var entity = await repo.GetByIdAsync(dto.RegistroId);

            if (entity == null)
                throw new EntityNotFoundException("Registro no encontrado.");

           // await GenerarAuditoriaAsync(dto, entity);

            entity.fecha = dto.Fecha;
            entity.horas = dto.Horas;
            entity.id_periodo = dto.PeriodoId;
            entity.codigo = dto.Codigo;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }





    private async Task GenerarAuditoriaAsync(RegistroDto dto, Registro registroOriginal)
        {


            if (registroOriginal == null)
                throw new EntityNotFoundException(
                    "Registro no encontrado."
                );

            var managerClaim = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirst("sub");

            if (
                managerClaim == null
                || !int.TryParse(managerClaim.Value, out int managerId)
            )
                throw new UnauthorizedException(
                    "Usuario no autenticado."
                );

     

            string cambios = "UPDATE";

            if (registroOriginal.horas != dto.Horas)
                cambios +=
                    $" | Horas: {registroOriginal.horas} -> {dto.Horas}";

            if (registroOriginal.codigo != dto.Codigo)
                cambios +=
                    $" | Codigo: {registroOriginal.codigo} -> {dto.Codigo}";

            if (registroOriginal.id_periodo != dto.PeriodoId)
                cambios +=
                    $" | Periodo: {registroOriginal.id_periodo} -> {dto.PeriodoId}";

            if (registroOriginal.fecha != dto.Fecha)
                cambios +=
                    $" | Fecha: {registroOriginal.fecha:d} -> {dto.Fecha:d}";

            var auditoria = new Auditoria
            {
                id_registro = registroOriginal.id_registro,
                fecha = DateOnly.FromDateTime(dto.Fecha),
                horas = dto.Horas,
                no_usuario = registroOriginal.no_usuario,
                id_periodo = dto.PeriodoId,
                codigo = dto.Codigo,
                //
                accion =  cambios,
                usuario_accion = managerId,
                fecha_accion = DateTime.UtcNow
            };

            await _unitOfWork
                .Repository<Auditoria>()
                .AddAsync(auditoria);

            await _unitOfWork
                .SaveChangesAsync();
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
            if (horas < 0)
                throw new BadRequestException("Las horas no pueden ser negativas.");
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
                if (accion == "Registrar" && rolUsuario != "Empleado")
                    throw new UnauthorizedException("Solo los empleados pueden registrar horas.");

                if (accion == "Modificar" && rolUsuario != "Manager")
                    throw new UnauthorizedException("No tiene autorizacion.");

                return;
            }

            if (ahora >= diaBloqueoInicio && ahora <= diaBloqueoFin)
                throw new BadRequestException("El periodo está en cierre inicial. Nadie puede modificar registros durante este día.");

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

        public async Task ValidarProyectoAsync(string codigo)
        {
            var proyecto = await _unitOfWork.Repository<Proyecto>().GetAsync(p => p.codigo == codigo);

            if (proyecto == null)
                throw new EntityNotFoundException("El proyecto no existe.");

            if (proyecto.id_empresa <= 0)
                throw new BadRequestException("El proyecto no tiene empresa asignada.");

            if (!proyecto.codigo.StartsWith("L", StringComparison.OrdinalIgnoreCase) && !proyecto.codigo.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("El código del proyecto no tiene clasificación válida.");
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

        public async Task RegistrarAsync(RegistroCreateDto dto, string rolUsuario)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(dto.PeriodoId);

            if (periodo == null)
                throw new EntityNotFoundException($"Periodo Incorrecto");

            _validacion.ValidarAccesoPeriodo(periodo, rolUsuario, "Registrar");

            var duplicado = await _unitOfWork.Repository<Registro>().FirstOrDefaultAsync(r => r.no_usuario == dto.NoUsuario && r.fecha.Date == dto.Fecha.Date && r.codigo == dto.Codigo);

            if (duplicado != null)
                throw new ConflictException("Registro duplicado, Intentelo de nuevo");

            var registro = new Registro
            {
                fecha = dto.Fecha,
                horas = dto.Horas,
                no_usuario = dto.NoUsuario,
                id_periodo = dto.PeriodoId,
                codigo = dto.Codigo
            };

            await _unitOfWork.Repository<Registro>().AddAsync(registro);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditarAsync(RegistroDto dto, string rolUsuario)
        {
            _validacion.ValidarHoras(dto.Horas);

            await _validacion.ValidarProyectoAsync(dto.Codigo);

            var registro = await _unitOfWork.Repository<Registro>().GetByIdAsync(dto.RegistroId);

            if (registro == null)
                throw new EntityNotFoundException("Registro no encontrado.");

            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(dto.PeriodoId);

            if (periodo == null)
                throw new EntityNotFoundException($"El periodo con ID {dto.PeriodoId} no existe.");

            _validacion.ValidarAccesoPeriodo(periodo, rolUsuario, "Modificar");

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

        public async Task<decimal> GetHorasSemanaAsync(int noUsuario, int periodoId)
        {
            var periodo = await _unitOfWork.Repository<Periodo>().GetByIdAsync(periodoId);

            if (periodo == null)
                throw new EntityNotFoundException($"El periodo {periodoId} no existe.");

            // Semana actual: lunes a domingo
            var hoy = DateTime.Now.Date;
            var diasDesdelunes = ((int)hoy.DayOfWeek + 6) % 7; // lunes = 0
            var inicioSemana = hoy.AddDays(-diasDesdelunes);
            var finSemana = inicioSemana.AddDays(6);

            var registros = await _unitOfWork.Repository<Registro>().GetAllAsync();

            return registros
                .Where(r => r.no_usuario == noUsuario
                         && r.fecha.Date >= inicioSemana
                         && r.fecha.Date <= finSemana)
                .Sum(r => r.horas);
        }

        // METODO QUE CONECTA A LA TABLA BD DE USUARIOS
        public async Task<List<MEmpleadoDto>> ObtenerTop3EmpleadosHoras(DateTime fechaInicio)
        {
            var usuarios = await _unitOfWork.Repository<Usuario>().GetAllAsync();

            var m_empleado = usuarios
                .Take(3)
                .Select(u => new MEmpleadoDto
                {
                    IdUsuario = u.no_usuario,
                    NombreEmpleado = $"{u.nombre} {u.ap_paterno}",
                    TotalHoras = 999
                })
                .ToList();

            return m_empleado;
        }



        // METODO PARA OBTENER LOS 3 EMPLEADOS CON MAS HORAS (CONECTADO A LA TABLA DE REGISTRO)
        //public async Task<List<MEmpleadoDto>> ObtenerTop3EmpleadosHoras(DateTime fechaInicio)
        //{
        //DateTime fechaFin = fechaInicio.AddDays(14);

        // OBTENER REGISTROS
        //var registros = await _unitOfWork.Repository<Registro>().GetAllAsync();

        // FILTRAR SOLO LOS REGISTROS DEL PERIODO
        //var registrosPeriodo = registros.Where(r => r.fecha >= fechaInicio && r.fecha < fechaFin).ToList();

        // SI NO HAY REGISTROS
        //if (!registrosPeriodo.Any())
        // return new List<MEmpleadoDto>();

        // OBTENER USUARIOS
        //var usuarios = await _unitOfWork.Repository<Usuario>().GetAllAsync();

        // GENERAR TOP 3
        //var topEmpleados = registrosPeriodo
        //.GroupBy(r => r.no_usuario)
        //.Select(g =>
        //{
        //var usuario = usuarios.FirstOrDefault(u => u.no_usuario == g.Key);

        //return new MEmpleadoDto
        //{
        //IdUsuario = g.Key,

        //NombreEmpleado = usuario != null
        // ? $"{usuario.nombre} {usuario.ap_paterno}"
        // : $"Empleado {g.Key}",

        //TotalHoras = g.Sum(x => x.horas)
        //};
        // })
        // .OrderByDescending(x => x.TotalHoras)
        // .Take(3)
        // .ToList();

        // return topEmpleados;
        //}
    }
}