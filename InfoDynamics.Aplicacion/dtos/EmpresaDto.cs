using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;


namespace InfoDynamics.Aplicacion.dtos
{
    public class EmpresaDto

{

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;


    [Required, RegularExpression("NoCobrable|Cobrable|Ambos")]
    public string TipoProyecto { get; set; } = null!;

}

public class EmpresaProyectoResponseDto
{
    [Required]
    public int IdEmpresa { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    public string? CodigoCobrable { get; set; }

    public string? CodigoNoCobrable { get; set; }
}

}

