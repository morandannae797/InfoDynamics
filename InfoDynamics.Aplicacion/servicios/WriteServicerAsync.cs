using AutoMapper;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;

namespace InfoDynamics.Aplicacion.servicio
{

    
    public class WriteServiceAsync<TEntity, TCreateDto, TUpdateDto>
     : IWriteServiceAsync<TCreateDto, TUpdateDto>
     where TEntity : class
     where TCreateDto : class
     where TUpdateDto : class//, IConcurrencyDto
    {
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;

        public WriteServiceAsync(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual async Task AddAsync(TCreateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _unitOfWork.Repository<TEntity>().AddAsync(entity);


            await _unitOfWork.SaveChangesAsync();

        }

        public virtual async Task UpdateAsync(TUpdateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _unitOfWork.Repository<TEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _unitOfWork.Repository<TEntity>().DeleteByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class WriteServiceSingleAsync<TEntity, TDto>
     : IWriteServiceSingleAsync<TDto>
     where TEntity : class
     where TDto : class
    {
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;

        public WriteServiceSingleAsync(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual async Task AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _unitOfWork.Repository<TEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _unitOfWork.Repository<TEntity>().UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _unitOfWork.Repository<TEntity>().DeleteByIdAsync(id);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}