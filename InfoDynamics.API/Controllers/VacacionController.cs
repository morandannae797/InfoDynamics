using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq; // NECESARIO PARA LOS FILTROS

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacacionController : ControllerBase
    {
        private readonly IReadServiceAsync<VacacionDto.VacacionResponseDto> _readService;
        private readonly IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto> _writeService;
        private readonly IVacacionAprobacionService _vacacionAprobacionService;
        private readonly IUnitOfWork _unitOfWork;

        public VacacionController(
            IReadServiceAsync<VacacionDto.VacacionResponseDto> readService,
            IWriteServiceAsync<VacacionDto.VacacionCreateDto, VacacionDto.VacacionAprobacionDto> writeService,
            IVacacionAprobacionService vacacionAprobacionService,
            IUnitOfWork unitOfWork)
        {
            _readService = readService;
            _writeService = writeService;
            _vacacionAprobacionService = vacacionAprobacionService;
            _unitOfWork = unitOfWork;
        }

        // 1. MANAGER: Ve solo a sus empleados a cargo
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacacionDto.VacacionResponseDto>>> GetAll()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized("Usuario no autenticado.");
            int managerId = int.Parse(claim.Value);

            // Obtener IDs de usuarios a cargo
            var todasLasRelaciones = await _unitOfWork.Repository<Usuario_manager>().GetAllAsync();
            var usuariosACargo = todasLasRelaciones
                .Where(x => x.no_usuario_manager == managerId)
                .Select(x => x.no_usuario)
                .ToList();

            // Obtener vacaciones y filtrar
            var todasLasVacaciones = await _readService.GetAllAsync();
            var filtradas = todasLasVacaciones
                .Where(v => usuariosACargo.Contains(v.SolicitanteId))
                .ToList();

            return Ok(filtradas);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VacacionDto.VacacionResponseDto>> GetById(int id)
        {
            try
            {
                return Ok(await _readService.GetByIdAsync(id));
            }
            catch (EntityNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        // 2. EMPLEADO: Crea solicitud con validación de "Una pendiente a la vez"
        [Authorize(Roles = "Empleado")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] VacacionDto.VacacionCreateDto dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized("Usuario no autenticado.");
            int usuarioId = int.Parse(claim.Value);

            var solicitudExistente = await _unitOfWork.Repository<Vacacion>()
                .GetAsync(v => v.no_usuario == usuarioId && v.estado == "Pendiente");

            if (solicitudExistente != null)
                return Conflict(new { message = "Ya tienes una solicitud de vacaciones pendiente." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            dto.SolicitanteId = usuarioId;
            await _writeService.AddAsync(dto);

            return Ok(new { mensaje = "Vacación solicitada correctamente." });
        }

        // 3. MANAGER: Evalúa solicitudes
        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/evaluar")]
        public async Task<IActionResult> EvaluarVacacion(int id, [FromBody] VacacionDto.VacacionAprobacionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                int noUsuarioManager = int.Parse(claim.Value);

                await _vacacionAprobacionService.EvaluarVacacionAsync(id, dto, noUsuarioManager);
                return Ok(new { mensaje = "Estado de la vacación actualizado." });
            }
            catch (Exception ex) when (ex is EntityNotFoundException || ex is UnauthorizedException || ex is ConflictException)
            {
                return StatusCode(ex is UnauthorizedException ? 403 : 404, new { message = ex.Message });
            }
        }
    }
}