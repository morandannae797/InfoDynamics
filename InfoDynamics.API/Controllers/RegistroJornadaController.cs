//
using System.Security.Claims;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroJornadaController : ControllerBase
    {
        private readonly IReadServiceAsync<RegistroDto> _readService;
        private readonly IWriteServiceAsync<RegistroCreateDto, RegistroDto> _writeService;
        private readonly JornadaCalculoService _jornadaCalculoService;
        //
        private readonly JornadaRegistroService _jornadaRegistroService;

        public RegistroJornadaController(
            IReadServiceAsync<RegistroDto> readService,
            IWriteServiceAsync<RegistroCreateDto, RegistroDto> writeService,
            JornadaCalculoService jornadaCalculoService,
            //
            JornadaRegistroService jornadaRegistroService)
        {
            _readService = readService;
            _writeService = writeService;
            _jornadaCalculoService = jornadaCalculoService;
            //
            _jornadaRegistroService = jornadaRegistroService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroDto>>> GetAll()
        {
            try
            {
                var registros = await _readService.GetAllAsync();
                return Ok(registros);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegistroDto>> GetById(int id)
        {
            try
            {
                var registro = await _readService.GetByIdAsync(id);
                return Ok(registro);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpGet("usuario/{noUsuario:int}")]
        public async Task<ActionResult<IEnumerable<RegistroDto>>> GetByUsuario(int noUsuario)
        {
            try
            {
                var registros = await _readService.GetAllAsync();
                var resultado = registros.Where(r => r.NoUsuario == noUsuario).ToList();

                if (!resultado.Any())
                    return NotFound(new { message = "No se encontraron registros para ese usuario." });

                return Ok(resultado);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RegistroCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "No se pudo determinar el rol del usuario." });

            try
            {
                await _jornadaRegistroService.RegistrarAsync(dto, rol);
                return Ok(new { message = "Registro de jornada creado correctamente." });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        /*[Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RegistroCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _writeService.AddAsync(dto);

            return Ok(new { message = "Registro de jornada creado correctamente." });
        }
        */
        [Authorize]
        [HttpPost("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] RegistroDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.RegistroId)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "No se pudo determinar el rol del usuario." });

            if (rol != "Manager")
                return Unauthorized(new { message = "Solo los managers pueden modificar registros." });

            try
            {
                await _writeService.UpdateAsync(dto);
                return Ok(new { message = "Registro modificado correctamente." });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "El registro fue modificado por otro proceso. Recarga los datos y reintenta." });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        //OBTENER EMPLEADOS
        [Authorize]
        [HttpGet("top-empleados")]
        public async Task<ActionResult<IEnumerable<MEmpleadoDto>>> ObtenerTop3Empleados([FromQuery] DateTime fechaInicio)
        {
            // VALIDAR FECHA
            if (fechaInicio == DateTime.MinValue)
            {
                return BadRequest("Debe enviar una fecha válida.");
            }

            var resultado = await _jornadaCalculoService
                .ObtenerTop3EmpleadosHoras(fechaInicio);

            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("horas-semana/{noUsuario}/{periodoId}")]
        public async Task<IActionResult> GetHorasSemana(int noUsuario, int periodoId)
        {
            var total = await _jornadaCalculoService.GetHorasSemanaAsync(noUsuario, periodoId);
            return Ok(total);
        }
    }
}