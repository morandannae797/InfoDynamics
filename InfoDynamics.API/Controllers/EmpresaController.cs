using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.Servicios.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly IReadServiceAsync<EmpresaDto> _readService;
        private readonly EmpresaRegistroService _empresaService;


        public EmpresaController(
            IReadServiceAsync<EmpresaDto> readService,
            EmpresaRegistroService empresaService)
        {
            _readService = readService;
            _empresaService = empresaService;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpresaProyectoResponseDto>>> GetAll()
        {
            try
            {
                var empresas = await _empresaService.GetEmpresasConCodigosAsync();
                return Ok(empresas);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("{id:int}")]
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

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] EmpresaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _empresaService.CreateAsync(dto);

                return Ok(new
                {
                    message = "Empresa creada correctamente."
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("update/{id:int}")]
        public async Task<ActionResult> Update(
            int id,
            [FromBody] EmpresaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _empresaService.UpdateNombreYAgregarCodigoAsync(id, dto);

                return Ok(new
                {
                    message = "Empresa actualizada y código agregado correctamente."
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}