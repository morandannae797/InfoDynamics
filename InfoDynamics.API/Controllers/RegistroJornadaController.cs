using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroJornadaController : ControllerBase
    {
        private readonly IReadServiceAsync<RegistroResponseDto> _readService;
        private readonly IWriteServiceAsync<RegistroCreateDto, RegistroUpdateDto> _writeService;

        public RegistroJornadaController(
            IReadServiceAsync<RegistroResponseDto> readService,
            IWriteServiceAsync<RegistroCreateDto, RegistroUpdateDto> writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetAll()
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegistroResponseDto>> GetById(int id)
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

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RegistroCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _writeService.AddAsync(dto);

            return Ok(new { message = "Registro de jornada creado correctamente." });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] RegistroUpdateDto dto)
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _writeService.DeleteAsync(id);
            return NoContent();
        }
    }
}