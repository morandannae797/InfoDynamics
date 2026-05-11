using System.ComponentModel.DataAnnotations;

public class EmpresaDto
{
    [Required]
    public int IDEmpresa { get; set; }

    public string Nombre { get; set; } = null!;

    public string descripcion { get; set; }

    public byte[]? RowVersion { get; set; } = null!;
}