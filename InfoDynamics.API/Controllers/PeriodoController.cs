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
    public class PeriodoController : ControllerBase
    {
        private readonly IReadServiceAsync<PeriodoDto> _readService;
        private readonly IWriteServiceAsync<PeriodoDto, PeriodoDto> _writeService;
        private readonly PeriodoService _periodoService;

        public PeriodoController(IReadServiceAsync<PeriodoDto> readService, IWriteServiceAsync<PeriodoDto, PeriodoDto> writeService, PeriodoService periodoService)
        {
            _readService = readService;
            _writeService = writeService;
            _periodoService = periodoService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodoDto>>> GetAll()
        {
            try
            {
                var periodos = await _readService.GetAllAsync();
                return Ok(periodos);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeriodoDto>> GetById(int id)
        {
            try
            {
                var periodo = await _readService.GetByIdAsync(id);
                return Ok(periodo);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("generar")]
        public async Task<ActionResult> GenerarPeriodo()
        {
            try
            {
                var periodo = await _periodoService.GenerarPeriodoAsync();

                return Ok(new
                {
                    message = "Periodo generado correctamente.",
                    periodo
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] PeriodoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.PeriodoId)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });

            try
            {
                await _writeService.UpdateAsync(dto);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "El periodo fue modificado por otro proceso. Recarga los datos y reintenta." });
            }
        }
    }
}