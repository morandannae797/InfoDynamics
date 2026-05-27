namespace InfoDynamics.Dominio.Entidades;

public partial class Proyecto
{
    public string codigo { get; set; } = null!;

    public bool es_cobrable { get; set; } 

    public int id_empresa { get; set; }


    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

    public virtual Empresa id_empresaNavigation { get; set; } = null!;
}
