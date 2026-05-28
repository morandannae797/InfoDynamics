namespace InfoDynamics.Dominio.Entidades
{
    public class Vacacion
    {
        public int id_vacacion { get; set; }

        public DateOnly fecha_inicio { get; set; }

        public DateOnly fecha_fin { get; set; }

        public string estado { get; set; } = null!;

        public int no_usuario { get; set; }
    }
}