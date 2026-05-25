using InfoDynamics.Dominio.interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoDynamics.Aplicacion.dtos
{
    public class UsuarioResponseDTO
    {
        public int NoUsuario { get; set; }

        public string NoUsuarioFormateado => NoUsuario.ToString("D7");

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string ApPaterno { get; set; } = null!;

        public string? ApMaterno { get; set; }

        public string NombreCompleto => $"{Nombre} {ApPaterno} {ApMaterno}".Trim();

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public bool EsManager { get; set; }

        [Required]
        public bool EstadoCuenta { get; set; }

        [Required]
        public bool DebeCambiarPass { get; set; }

        [Required]
        public int Intentos { get; set; }

        public DateTime? HoraBloqueo { get; set; }

        public string? Token { get; set; }

        [Required]
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

        [Required]
        [MinLength(12, ErrorMessage = "La contraseña debe tener 12 caracteres como mínimo.")]
        public string Contrasena { get; set; } = null!;

        [Required]
        [Column("es_admin")]
        public bool EsManager { get; set; } = false;
    }

    public class UsuarioUpdateDto : IConcurrencyDto
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

        [Required]
        [Column("es_admin")]
        public bool EsManager { get; set; } = false;

        [Required]
        public bool EstadoCuenta { get; set; }

        [Required]
        public bool DebeCambiarPass { get; set; }

        public string? Token { get; set; }

        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }

    public class UsuarioDesactivarDto : IConcurrencyDto
    {
        [Required]
        public byte[] RowVersion { get; set; } = null!;
    }

    public class CambiarContrasenaDto
    {
        [Required]
        public string ContrasenaActual { get; set; } = null!;

        [Required]
        [MinLength(12, ErrorMessage = "La nueva contraseña debe tener 12 caracteres como mínimo.")]
        public string NuevaContrasena { get; set; } = null!;

        [Required]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarNuevaContrasena { get; set; } = null!;
    }
    public class RestablecerContrasenaDto
    {
        [Required]
        [MinLength(12, ErrorMessage = "La nueva contraseña debe tener 12 caracteres como mínimo.")]
        public string NuevaContrasena { get; set; } = null!;

        [Required]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarNuevaContrasena { get; set; } = null!;
    }
}