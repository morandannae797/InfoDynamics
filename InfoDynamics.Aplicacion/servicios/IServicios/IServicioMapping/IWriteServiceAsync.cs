namespace InfoDynamics.Aplicacion.servicio.IServicios
{
    public interface IWriteServiceAsync< TDto> 
       
        where TDto : class

    {
        Task AddAsync(TDto dto);
        Task DeleteAsync(int id);
        Task UpdateAsync(TDto dto);
    }
}