using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfoDynamics.Dominio.Entidades { 

public partial class Empresa
{public int id_empresa { get; set; }

    public string nombre { get; set; } = null!;

    public string descripcion { get; set; }
        [Timestamp]
        public byte [] RowVersion { get; set; }= null!;
    }
}