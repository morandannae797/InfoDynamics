using InfoDynamics.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoDynamics.Dominio.Entidades
{
    public partial class Usuario
{

    public int no_usuario { get; set; }

    public string nombre { get; set; } = null!;

    public string ap_paterno { get; set; } = null!;

    public string? ap_materno { get; set; }

    public string email { get; set; } = null!;
    
    [Column("es_admin")]
    public bool es_manager { get; set; }

    public bool estado_cuenta { get; set; }

    public string contrasena_hash { get; set; } = null!;

    public bool debe_cambiar_pass { get; set; }

    public int intentos { get; set; }

    public DateTime? hora_bloqueo { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

 
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();


    public virtual ICollection<HistorialContrasena> Contrasenas { get; set; } = new List<HistorialContrasena>();
    }

}