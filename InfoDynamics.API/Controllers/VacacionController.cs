using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacacionController : ControllerBase
    {
        private readonly IReadServiceAsync<VacacionDto.VacacionResponseDto> _readService;
        private readonly IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto> _writeService;
        private readonly IVacacionAprobacionService _vacacionAprobacionService;

        public VacacionController(
            IReadServiceAsync<VacacionDto.VacacionResponseDto> readService,
            IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto> writeService,
            IVacacionAprobacionService vacacionAprobacionService)
        {
            _readService = readService;
            _writeService = writeService;
            _vacacionAprobacionService = vacacionAprobacionService;
        }

        [Authorize(Roles = "Manager")]
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

        [Authorize]
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

        [Authorize(Roles = "Empleado")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] VacacionDto.VacacionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedException("Usuario no autenticado.");

            dto.SolicitanteId = int.Parse(claim.Value);

            await _writeService.AddAsync(dto);

            return Ok(new { mensaje = "Vacacion solicitada correctamente." });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/evaluar")]
        public async Task<IActionResult> EvaluarVacacion(int id, [FromBody] VacacionDto.VacacionAprobacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedException("Usuario no autenticado.");

                int noUsuarioManager = int.Parse(claim.Value);

                await _vacacionAprobacionService.EvaluarVacacionAsync(id, dto, noUsuarioManager);

                return Ok(new { mensaje = "Estado de la vacacion actualizado exitosamente." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "La solicitud fue modificada por otro proceso. Recarga los datos y reintenta." });
            }
        }
    }
}