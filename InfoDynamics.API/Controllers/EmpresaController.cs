using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly IReadServiceAsync<EmpresaResponseDto> _readService;
        private readonly IWriteServiceAsync<EmpresaCreateDto, EmpresaUpdateDto> _writeService;

        public EmpresaController(
            IReadServiceAsync<EmpresaResponseDto> readService,
            IWriteServiceAsync<EmpresaCreateDto, EmpresaUpdateDto> writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpresaResponseDto>>> GetAll()
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmpresaResponseDto>> GetById(int id)
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
        public async Task<ActionResult> Create([FromBody] EmpresaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _writeService.AddAsync(dto);

            return Ok(new { message = "Empresa creada correctamente." });
        }

        [HttpPost("update/{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] EmpresaUpdateDto dto)
        {
            if (id != dto.IdEmpresa)
                return BadRequest("El id de la URL no coincide con el id del body.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.IdEmpresa)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });
           
            try
            {
                await _writeService.UpdateAsync(dto);
                return Ok("Actualizado correctamente.");
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("El registro fue modificado por otro usuario.");
            }
        }

       
    }
}