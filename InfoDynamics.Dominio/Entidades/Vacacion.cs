namespace InfoDynamics.Dominio.Entidades
{
    public class Vacacion
    {
        public int id_vacacion { get; set; }

        public DateOnly fecha_inicio { get; set; }

        public DateOnly fecha_fin { get; set; }

        // Inicializamos directamente como "Pendiente"
        public string estado { get; set; } = "Pendiente";

        public int no_usuario { get; set; }
    }
}