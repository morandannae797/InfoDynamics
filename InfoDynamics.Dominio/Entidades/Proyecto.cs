using System;
using System.Collections.Generic;
using InfoDynamics.Dominio.Entidades;

namespace InfoDynamics.Dominio.Entidades;

public partial class Proyecto
{
    public int id_proyecto { get; set; }

    public string codigo { get; set; } = null!;

    public string categoria { get; set; } = null!;

    public int id_empresa { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

    public virtual Empresa id_empresaNavigation { get; set; } = null!;
}
