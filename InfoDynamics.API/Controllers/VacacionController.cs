using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacacionController : ControllerBase
    {
        private readonly IReadServiceAsync<VacacionDto.VacacionResponseDto> _readService;
        private readonly IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto> _writeService;

        public VacacionController(
            IReadServiceAsync<VacacionDto.VacacionResponseDto> readService,
            IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto> writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacacionDto.VacacionResponseDto>>> GetAll()
        {
            try
            {
                var vacaciones = await _readService.GetAllAsync();
                return Ok(vacaciones);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VacacionDto.VacacionResponseDto>> GetById(int id)
        {
            try
            {
                var vacacion = await _readService.GetByIdAsync(id);
                return Ok(vacacion);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] VacacionDto.VacacionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _writeService.AddAsync(dto);

            return Ok(new { mensaje = "Vacación solicitada correctamente." });
        }

        [HttpPost("{id:int}/evaluar")]
        public async Task<IActionResult> EvaluarVacacion(int id, [FromBody] VacacionDto.VacacionAprobacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.VacacionId)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });

            try
            {
                var vacacion = await _readService.GetByIdAsync(id);

                if (vacacion.EstadoAprobacion != "Pendiente")
                    return BadRequest(new { mensaje = "Esta solicitud ya fue evaluada anteriormente." });

                await _writeService.UpdateAsync(dto);

                return Ok(new { mensaje = "Estado de la vacación actualizado exitosamente." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "La solicitud fue modificada por otro proceso. Recarga los datos y reintenta." });
            }
        }
    }
}