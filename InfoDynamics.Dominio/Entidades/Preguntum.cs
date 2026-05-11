using System;
using System.Collections.Generic;
using InfoDynamics.Dominio.Entidades;

namespace InfoDynamics.Dominio.Entidades;

public partial class Preguntum
{
    public int id_pregunta { get; set; }

    public string pregunta { get; set; } = null!;

    public string respuesta { get; set; } = null!;

    public int no_usuario { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Usuario no_usuarioNavigation { get; set; } = null!;
}
