using InfoDynamics.Aplicacion.dtos;

namespace InfoDynamics.Aplicacion.servicio.IServicios
{
    public interface IVacacionAprobacionService
    {
        Task EvaluarVacacionAsync(int idVacacion, VacacionDto.VacacionAprobacionDto dto, int noUsuarioManager);
    }
}