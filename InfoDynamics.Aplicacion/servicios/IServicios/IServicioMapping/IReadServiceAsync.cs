namespace InfoDynamics.Aplicacion.servicio.IServicios
{
    public interface IReadServiceAsync< TDto> where TDto : class
    {
       Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id); 
    }

}