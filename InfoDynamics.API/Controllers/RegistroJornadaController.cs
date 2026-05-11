using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;

using InfoDynamics.Aplicacion.servicio.IServicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoDynamics.Aplicacion.Controllers {
  //  [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class RegistroJornadaController : ControllerBase
    {
        private readonly IReadServiceAsync<RegistroJornadaDto>_readService;
        private readonly IWriteServiceAsync<RegistroJornadaDto> _writeService;

        public RegistroJornadaController(
                    IReadServiceAsync<RegistroJornadaDto> readService,
                    IWriteServiceAsync<RegistroJornadaDto> writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }


        [HttpGet]

        public async Task<ActionResult<IEnumerable<RegistroJornadaDto>>> GetAll()
        {
            try
            {
                var registros = await _readService.GetAllAsync();
                return Ok(registros); // Devuelve 200 OK
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // Devuelve 404
            }
        }
[ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegistroJornadaDto>> GetById(int id)
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
        public async Task<ActionResult> Create([FromBody] RegistroJornadaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve 400 si el DTO no cumple las validaciones
            }

            await _writeService.AddAsync(dto);

            // Lo ideal en un POST es devolver 201 
            return CreatedAtAction(nameof(GetById), new { id = dto.RegistroID }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] RegistroJornadaDto dto)
        {
            if (id != dto.RegistroID)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });
            }

            // Aquí podrías envolver en un try-catch por si el registro no existe
            await _writeService.UpdateAsync(dto);

            return NoContent(); // Devuelve 204 No Content (estándar para un PUT exitoso)
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _writeService.DeleteAsync(id);
            return NoContent(); // Devuelve 204 No Content
        }
        



    } 
}