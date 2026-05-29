using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

public class EmpresaDto

{

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;


    [Required]
    public string TipoProyecto { get; set; } = null!;

}

