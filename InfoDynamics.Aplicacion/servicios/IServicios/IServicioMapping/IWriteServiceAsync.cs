namespace InfoDynamics.Aplicacion.servicio.IServicios
{
    public interface IWriteServiceAsync<TCreateDto, TUpdateDto>
       where TCreateDto : class
       where TUpdateDto : class
    {
        Task AddAsync(TCreateDto dto);
        Task UpdateAsync(TUpdateDto dto);
        Task DeleteAsync(int id);
    }
}