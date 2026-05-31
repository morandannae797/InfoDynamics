using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager")]
public class AuditoriaController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditoriaController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("registro/{id}")]
    public async Task<IActionResult> GetByRegistro(int id)
    {
        var auditorias = await _unitOfWork
            .Repository<Auditoria>()
            .GetAllAsync();

        var filtradas = auditorias
            .Where(a => a.id_registro == id)
            .ToList();

        return Ok(filtradas);
    }
}