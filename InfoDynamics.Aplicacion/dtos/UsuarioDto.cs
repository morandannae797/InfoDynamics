using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class UsuarioResponseDTO
    {
        public int NoUsuario { get; set; }

        public string NoUsuarioFormateado => NoUsuario.ToString("D5");

        public string Nombre { get; set; } = null!;

        public string ApPaterno { get; set; } = null!;

        public string? ApMaterno { get; set; }

        public string NombreCompleto => $"{Nombre} {ApPaterno} {ApMaterno}".Trim();

        public string Email { get; set; } = null!;

        public string Rol { get; set; } = null!;

        public string EstadoCuenta { get; set; } = null!;

        public byte[] RowVersion { get; set; } = null!;
    }
    public class UsuarioCreateDTO 
    {
        [Required]
        public int NoUsuario { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, StringLength(50)]
        public string ApPaterno { get; set; } = null!;

        [StringLength(50)]
        public string? ApMaterno { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = null!;

        [Required, MinLength(8, ErrorMessage = "La contraseña debe tener  8 caracteres minimo.")]
        public string Contrasena { get; set; } = null!;

        [Required] [RegularExpression("^(Administrador|Empleado)$")]
        public string Rol { get; set; } = null!;
    }
    public class UsuarioUpdateDto 
    {
        [Required]
        public int NoUsuario { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, StringLength(50)]
        public string ApPaterno { get; set; } = null!;

        [StringLength(50)]
        public string? ApMaterno { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = null!;

        [Required, RegularExpression("Administrador|Empleado")]
        public string Rol { get; set; } = null!;

        [Required, RegularExpression("Activa|Bloqueada|Desactivada")]
        public string EstadoCuenta { get; set; } = null!;

        [Required]
        public byte[] RowVersion { get; set; } = null!;
       
        }

    public class UsuarioDesactivarDto 
    {
        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }
}