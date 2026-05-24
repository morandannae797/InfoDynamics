using InfoDynamics.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace InfoDynamics.Dominio.Entidades;

public partial class HistorialContrasena
{
    public int id_historial { get; set; }

    public string contrasena_hash { get; set; } = null!;

    public DateTime fecha_registro { get; set; }

    public bool es_temporal { get; set; }

    public int no_usuario { get; set; }

    public byte[] RowVersion { get; set; } = null!;
    public virtual Usuario no_usuarioNavigation { get; set; } = null!;
}
