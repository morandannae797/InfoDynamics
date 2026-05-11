using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Infraestructura.Processors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Employees.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class LogInController : ControllerBase
    {
        private readonly IHmacServicio _HmacServicio;
        private readonly IAccountService _accountService;

        public LogInController(IHmacServicio hmacServicio, IAccountService accountService  )
        {
            _HmacServicio = hmacServicio;
            _accountService = accountService;
           

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(
        [FromBody] loginDto request,
        [FromHeader(Name = "firma")] string signature)
        {
            if (request == null)
                return BadRequest(new { message = "El cuerpo de la petición no puede estar vacío." });

            if (string.IsNullOrWhiteSpace(signature))
                return BadRequest(new { message = "El header 'firma' es requerido." });

            if (!_HmacServicio.VerifySignature(request, signature))
                return Unauthorized(new { message = "Firma no válida." });

            await _accountService.LoginAsync(request);
            return Ok(new { message = "Inicio de sesión exitoso." });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["REFRESH_TOKEN"];
            await _accountService.RefreshtokenAsync(refreshToken);
            return Ok(new { message = "Token renovado." });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("ACCESS_TOKEN", new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
            Response.Cookies.Delete("REFRESH_TOKEN", new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
            return Ok(new { message = "Sesión cerrada con éxito." });
        }
    }
}
