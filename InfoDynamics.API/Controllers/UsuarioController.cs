using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly IContrasenaService _contrasenaService;
        private readonly IReadServiceAsync<UsuarioResponseDTO> _readService;
        private readonly Iusuarioservicio _usuarioServicio;

        public UsuarioController(

            IContrasenaService contrasenaService,
            IReadServiceAsync<UsuarioResponseDTO> readService,
            Iusuarioservicio usuarioServicio)
        {
            _contrasenaService = contrasenaService;
            _readService = readService;
            _usuarioServicio = usuarioServicio;
        }


        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetAll()
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

        [HttpGet("{id:int}")]
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


        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuario = await _usuarioServicio.CreateFromDtoAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = usuario.no_usuario },
                    new { message = "Usuario creado exitosamente." });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.NoUsuario)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del objeto." });

            try
            {
                var usuarioActualizado = new Usuario
                {
                    no_usuario = dto.NoUsuario,
                    nombre = dto.Nombre,
                    ap_paterno = dto.ApPaterno,
                    ap_materno = dto.ApMaterno,
                    email = dto.Email,
                    es_manager = dto.EsManager,
                    estado_cuenta = dto.EstadoCuenta
                };

                await _usuarioServicio.UpdateWithConcurrencyAsync(
                    usuarioActualizado,
                    dto.RowVersion);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "El usuario fue modificado por otro proceso. Recarga los datos y reintenta." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/desactivar")]
        public async Task<ActionResult> Desactivar(int id, [FromBody] UsuarioDesactivarDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuario = await _usuarioServicio.FindByIdAsync(id);

                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                usuario.estado_cuenta = false;

                await _usuarioServicio.UpdateWithConcurrencyAsync(
                    usuario,
                    dto.RowVersion);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("{id:int}/cambiar-contrasena")]
        public async Task<ActionResult> CambiarContrasena(int id, [FromBody] CambiarContrasenaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _contrasenaService.CambiarContrasenaAsync(
                    id,
                    dto.ContrasenaActual,
                    dto.NuevaContrasena,
                    dto.ConfirmarNuevaContrasena);

                return Ok(new { message = "Contraseña cambiada correctamente." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }


        [Authorize]
        [HttpPost("{id:int}/restablecer-contrasena")]
        public async Task<ActionResult> RestablecerContrasena(int id, [FromBody] RestablecerContrasenaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _contrasenaService.RestablecerContrasenaAsync(
                    id,
                    dto.NuevaContrasena,
                    dto.ConfirmarNuevaContrasena);

                return Ok(new { message = "Contraseña restablecida correctamente." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }


    }

    }