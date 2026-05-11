
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicio;
using Microsoft.AspNetCore.Mvc;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly IReadServiceAsync<EmpresaDto> _readService;
        private readonly IWriteServiceAsync<EmpresaDto> _writeService;

        public EmpresaController(IReadServiceAsync<EmpresaDto> readService, IWriteServiceAsync<EmpresaDto> writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }
        
        [HttpGet]
      
       
        public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetAll()
        {

            try
            {
                var empresas = await _readService.GetAllAsync();
                return Ok(empresas);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpresaDto>> GetById(int id)
        {
            try
            {
                var empresa = await _readService.GetByIdAsync(id);
                return Ok(empresa);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] EmpresaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _writeService.AddAsync(dto);
            // Asumiendo que EmpresaDto tiene una propiedad IDEmpresa según tu código anterior
            return CreatedAtAction(nameof(GetById), new { id = dto.IDEmpresa }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] EmpresaDto dto)
        {
            if (id != dto.IDEmpresa)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });

            await _writeService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _writeService.DeleteAsync(id);
            return NoContent();
        }
    }
}