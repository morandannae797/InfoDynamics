using AutoMapper;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoDynamics.API.Controllers
{
  //  [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VacacionController : ControllerBase
    {
        private readonly IReadServiceAsync<VacacionDto.VacacionResponseDTO> _readService;
        private readonly IWriteServiceAsync<VacacionDto.VacacionCreateDTO> _writeService;
        private readonly IWriteServiceAsync<VacacionDto.VacacionAprobacionDTO> _writeAprobacionService; 

        private readonly IMapper _mapper;

        public VacacionController(
    IReadServiceAsync<VacacionDto.VacacionResponseDTO> readService,
    IWriteServiceAsync<VacacionDto.VacacionCreateDTO> writeService,
    IWriteServiceAsync<VacacionDto.VacacionAprobacionDTO> writeAprobacionService, 
    IMapper mapper)
        {
            _readService = readService;
            _writeAprobacionService = writeAprobacionService;
            _writeService = writeService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacacionDto.VacacionResponseDTO>>> GetAll()
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

        [HttpGet("{id}")]
        public async Task<ActionResult<VacacionDto.VacacionResponseDTO>> GetById(int id)
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

        // --- COMMANDS 

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] VacacionDto.VacacionCreateDTO dto)
        {
            try
            {
                await _writeService.AddAsync(dto);
                return Ok(new { mensaje = "Vacación solicitada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] VacacionDto.VacacionCreateDTO dto)
        {
            try
            {
                await _writeService.UpdateAsync(dto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/evaluar")]
        public async Task<IActionResult> EvaluarVacacion(int id, [FromBody] VacacionDto.VacacionAprobacionDTO dto)
        {
            try
            {
                var vacacion = await _readService.GetByIdAsync(dto.VacacionId);

                if (vacacion == null)
                    return NotFound(new { mensaje = $"No se encontró la solicitud con ID {dto.VacacionId}" });

                if (vacacion.EstadoAprobacion != "Pendiente")
                    return BadRequest(new { mensaje = "Esta solicitud ya fue evaluada anteriormente." });

               
                await _writeAprobacionService.UpdateAsync(dto);

                return Ok(new { mensaje = "Estado de la vacación actualizado exitosamente." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
    }
