namespace InfoDynamics.Aplicacion.dtos
{
    public class RegistroJornadaDto
    {
        public int RegistroID { get; set; }

        public DateTime Fecha { get; set; }

        public decimal HorasTrabajadas { get; set; }

        public string Tipo { get; set; }

        public string Estado { get; set; }

        public int NumeroEmpleado { get; set; }

        public int PeriodoID { get; set; }

        public byte[]? RowVersion { get; set; } = null!;
    }
}
