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
        
       

        [HttpGet("testadminonly")]
        [Authorize (Roles = "Administrador")]
        public IActionResult Get()
        {
            var matrix = new List<string> { "matrix" };
            return Ok(matrix);
        }
    }

    }

