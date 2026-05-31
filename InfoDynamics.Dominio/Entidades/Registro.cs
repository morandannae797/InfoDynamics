using InfoDynamics.Dominio.Entidades;

public class Registro
{
    public int id_registro { get; set; }
    public DateTime fecha { get; set; }
    public decimal horas { get; set; }
    public int no_usuario { get; set; }
    public string codigo { get; set; }

    public int id_periodo { get; set; }

    // AGREGA ESTA LÍNEA:
    //public string id_proyecto { get; set; }

    public virtual Periodo id_periodoNavigation { get; set; } = null!;
    public virtual Proyecto codigoNavigation { get; set; } = null!;
    public virtual Usuario no_usuarioNavigation { get; set; } = null!;
}
