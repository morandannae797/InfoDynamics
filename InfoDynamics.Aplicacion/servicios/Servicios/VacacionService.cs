using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class VacacionService
    {
        private readonly DbContext _dbContext;

        public VacacionService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // CREAR VACACION
        public async Task AddAsync(VacacionDto.VacacionCreateDto dto)
        {
            // VALIDAR FECHAS
            if (dto.FechaFin < dto.FechaInicio)
            {
                throw new Exception("La fecha final no puede ser menor a la fecha inicial.");
            }

            // VALIDAR PERIODO ABIERTO
            var periodo = await _dbContext.Set<Periodo>()
                .FirstOrDefaultAsync(p =>
                    DateOnly.FromDateTime(dto.FechaInicio) >= p.fecha_inicio &&
                    DateOnly.FromDateTime(dto.FechaFin) <= p.fecha_fin &&
                    p.estado == "Abierto"
                );

            if (periodo == null)
            {
                throw new Exception("El período solicitado no es válido.");
            }

            // VALIDAR USUARIO
            var usuario = await _dbContext.Set<Usuario>()
                .FindAsync(dto.SolicitanteId);

            if (usuario == null)
            {
                throw new Exception("El solicitante no existe.");
            }

            // CREAR VACACION
            var vacacion = new Vacacion
            {
                fecha_solicito = DateTime.Now,

                fecha_inicio = DateOnly.FromDateTime(dto.FechaInicio),

                fecha_fin = DateOnly.FromDateTime(dto.FechaFin),

                estado = "Pendiente",

                no_usuario = dto.SolicitanteId,

                fecha_aprobo = null,

                id_administrador = null
            };

            // GUARDAR EN BD
            await _dbContext.Set<Vacacion>()
                .AddAsync(vacacion);

            await _dbContext.SaveChangesAsync();

            // SIMULACIONES
            Console.WriteLine("Solicitud enviada correctamente.");

            Console.WriteLine("Correo enviado al administrador.");
        }

        // APROBAR O RECHAZAR VACACION
        public async Task UpdateAsync(VacacionDto.VacacionAprobacionDto dto)
        {
            var vacacion = await _dbContext.Set<Vacacion>()
                .FirstOrDefaultAsync(v => v.id_vacacion == dto.VacacionId);

            if (vacacion == null)
            {
                throw new Exception("Vacación no encontrada.");
            }

            if (vacacion.estado != "Pendiente")
            {
                throw new Exception("La solicitud ya fue evaluada.");
            }

            // ACTUALIZAR
            vacacion.estado = dto.EstadoDecision;

            vacacion.id_administrador = dto.AprobadorId;

            vacacion.fecha_aprobo = DateTime.Now;

            // GUARDAR
            await _dbContext.SaveChangesAsync();

            // SIMULACION
            Console.WriteLine("Correo enviado al empleado.");
        }

        // METODO OBLIGATORIO
        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}