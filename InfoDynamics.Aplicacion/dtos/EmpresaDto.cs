using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

public class EmpresaCreateDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }
}
public class EmpresaUpdateDto : IConcurrencyDto
{
    [Required]
    public int IdEmpresa { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Required]
    public byte[] RowVersion { get; set; } = null!;
}

public class EmpresaResponseDto
{
    public int IdEmpresa { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
