using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Aplicacion.servicios.Servicios;
using InfoDynamics.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace InfoDynamics.API.Controllers

{
   // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IReadServiceAsync<UsuarioResponseDTO> _readService;
        private readonly IWriteServiceAsync<UsuarioCreateDTO> _writeService;
        private readonly Iusuarioservicio _usuarioServicio;

        public UsuarioController(
            IReadServiceAsync<UsuarioResponseDTO> readService, IWriteServiceAsync<UsuarioCreateDTO> writeService, Iusuarioservicio usuarioServicio)
        {
            _readService = readService;
            _writeService = writeService;
            _usuarioServicio = usuarioServicio;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioCreateDTO>>> GetAll()
        {
            try
            {
                var usuarios = await _readService.GetAllAsync();
                return Ok(usuarios);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDTO>> GetById(int id)
        {
            try
            {
                var usuario = await _readService.GetByIdAsync(id);
                return Ok(usuario);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _usuarioServicio.CreateFromDtoAsync(dto);

            return Ok(new { message = "Usuario creado exitosamente." });

        }

            [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UsuarioCreateDTO dto)
        {
            if (id != dto.no_usuario)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _writeService.UpdateAsync(dto);
            return NoContent();
        }
        //cambio a desactivar en lugar de eliminar, para mantener la integridad referencial y evitar problemas con datos relacionados., reescribir
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _writeService.DeleteAsync(id);
            return NoContent();
        }
    }
}