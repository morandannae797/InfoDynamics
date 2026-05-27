using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProyectoController : ControllerBase
    {
        private readonly IReadServiceAsync<ProyectoDto> _readService;
        private readonly IWriteServiceSingleAsync<ProyectoDto> _writeService;

        public ProyectoController(
            IReadServiceAsync<ProyectoDto> readService,
            IWriteServiceSingleAsync<ProyectoDto> writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProyectoDto>>> GetAll()
        {
            try
            {
                var proyectos = await _readService.GetAllAsync();

                return Ok(proyectos);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProyectoDto>> GetById(int id)
        {
            try
            {
                var proyecto = await _readService.GetByIdAsync(id);

                return Ok(proyecto);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult> Create(
            [FromBody] ProyectoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _writeService.AddAsync(dto);

            return Ok(new
            {
                message = "Proyecto creado correctamente."
            });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{codigo:int}")]
        public async Task<ActionResult> Update(string codigo, [FromBody] ProyectoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (codigo != dto.Codigo)
            {
                return BadRequest(new
                {
                    message = "El ID de la ruta no coincide con el del objeto."
                });
            }

            try
            {
                await _writeService.UpdateAsync(dto);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new
                {
                    message = "El proyecto fue modificado por otro proceso."
                });
            }
        }
    }
}