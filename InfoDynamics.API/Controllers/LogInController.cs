using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Dominio.Entidades;



namespace Employees.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
   
        private readonly IAccountService _accountService;


        public LogInController(
            IAccountService accountService)
        {
       
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] loginDto request)
        {


            if (request == null)
                return BadRequest(new { message = "El cuerpo de la peticion no puede estar vacío." });

            try
            {

                var user = await _accountService.LoginAsync(request);

                return Ok(new { message = "Inicio de sesion exitoso.",                       
                 user
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["REFRESH_TOKEN"];

            try
            {
                await _accountService.RefreshtokenAsync(refreshToken);
                return Ok(new { message = "Token renovado." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("ACCESS_TOKEN", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            Response.Cookies.Delete("REFRESH_TOKEN", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Sesion cerrada con exito." });
        }
    
    }

}