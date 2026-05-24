using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

public class EmpresaCreateDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

}
public class EmpresaUpdateDto : IConcurrencyDto
{
 
    public int IdEmpresa { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;


    [Required]
    public byte[] RowVersion { get; set; } = null!;
}

public class EmpresaResponseDto
{
    public int IdEmpresa { get; set; }

    public string Nombre { get; set; } = null!;


    public byte[] RowVersion { get; set; } = null!;
}
