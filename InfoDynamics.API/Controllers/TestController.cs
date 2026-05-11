using InfoDynamics.Aplicacion.servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employees.API.Controllers
{
  //  [Authorize]
    [ApiController]
        [Route("api/test")]
        public class TestController : ControllerBase
        {
        [HttpGet("test-hmac")]
        public IActionResult TestHmac([FromServices] IHmacServicio hmacServ)
        {
            return Ok(hmacServ != null ? "Servicio presente" : "Servicio nulo");
        }
        [HttpGet("omg")]
        [Authorize]  // Requiere autenticación igual que RequireAuthorization()
        public IActionResult Get()
        {
            var matrix = new List<string> { "matrix" };
            return Ok(matrix);
        }
    }

    }

