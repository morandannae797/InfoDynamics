using InfoDynamics.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace InfoDynamics.Dominio.Entidades;

public partial class Contrasena
{
    public int id_contrasena { get; set; }

    public string contrasena { get; set; } = null!;

    public DateTime fecha_creacion { get; set; }

    public string estado { get; set; } = null!;

    public bool es_temporal { get; set; }

    public int no_usuario { get; set; }

    public byte[] RowVersion { get; set; } = null!;
    public virtual Usuario no_usuarioNavigation { get; set; } = null!;
}
