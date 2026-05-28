using InfoDynamics.Aplicacion.dtos;

namespace InfoDynamics.Aplicacion.servicios.IServicios
{
    public interface IAuditoriaService
    {
        Task CrearAuditoriaAsync(AuditoriaDto dto);
    }
}