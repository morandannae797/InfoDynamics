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

        public RegistroJornadaController(
            IReadServiceAsync<RegistroDto> readService,
            IWriteServiceAsync<RegistroCreateDto, RegistroDto> writeService,
            JornadaCalculoService jornadaCalculoService)
        {
            _readService = readService;
            _writeService = writeService;
            _jornadaCalculoService = jornadaCalculoService;
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

            await _writeService.AddAsync(dto);

            return Ok(new { message = "Registro de jornada creado correctamente." });
        }

        [Authorize]

        [HttpPost("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] RegistroDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.RegistroId)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });
     
            try
            {
                await _writeService.UpdateAsync(dto);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "El registro fue modificado por otro proceso. Recarga los datos y reintenta." });
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