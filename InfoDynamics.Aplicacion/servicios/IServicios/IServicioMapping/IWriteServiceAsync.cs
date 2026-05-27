namespace InfoDynamics.Aplicacion.servicio.IServicios
{
    public interface IWriteServiceAsync<TCreateDto, TUpdateDto>
       where TCreateDto : class
       where TUpdateDto : class
    {
        Task AddAsync(TCreateDto dto);
        Task UpdateAsync(TUpdateDto dto);

    }

        public interface IWriteServiceSingleAsync<TDto>
            where TDto : class
        {
            Task AddAsync(TDto dto);

            Task UpdateAsync(TDto dto);

        }
  }
