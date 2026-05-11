using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Aplicacion.dtos
{
    public class UsuarioCreateDTO
    {
        [Required]
        public int no_usuario { get; set; }

        [Required, StringLength(100)]
        public string nombre { get; set; } = null!;

        [Required, StringLength(100)]
        public string ap_paterno { get; set; } = null!;

        [Required, StringLength(100)]
        public string ap_maternos { get; set; } = null!;

        [Required, EmailAddress]
        public string email { get; set; } = null!;

        [Required, MinLength(8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
        public string contrasena { get; set; } = null!;

        [Required, RegularExpression("Administrador|Empleado")]
        public string rol { get; set; } = null!;
    }

    public class UsuarioResponseDTO
    {
        private int _no_usuario;

        public int no_usuario
        {
            get => _no_usuario;
            set => _no_usuario = value;
        }

        public string no_usuarioFormateado => _no_usuario.ToString("D5");

        public byte[] RowVersion { get; set; } = null!;

        public string NombreCompleto => $"{nombre} {ap_paterno} {ap_maternos}";

        public string nombre { get; set; } = null!;
        public string ap_paterno { get; set; } = null!;
        public string ap_maternos { get; set; } = null!;

        public string email { get; set; } = null!;
        public string rol { get; set; } = null!;
    }
}