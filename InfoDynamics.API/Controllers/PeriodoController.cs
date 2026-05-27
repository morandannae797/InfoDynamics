using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeriodoController : ControllerBase
    {
        private readonly IReadServiceAsync<PeriodoResponseDto> _readService;
        //private readonly IWriteServiceAsync<PeriodoDto, PeriodoDto> _writeService;

        public PeriodoController(
            IReadServiceAsync<PeriodoResponseDto> readService)
            //IWriteServiceAsync<PeriodoCreateDto, PeriodoUpdateDto> writeService)
        {
            _readService = readService;
            //_writeService = writeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodoResponseDto>>> GetAll()
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeriodoResponseDto>> GetById(int id)
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
        /*
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] PeriodoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _writeService.AddAsync(dto);

            return Ok(new { message = "Periodo creado correctamente." });
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] PeriodoUpdateDto dto)
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

        */

    
    }
}