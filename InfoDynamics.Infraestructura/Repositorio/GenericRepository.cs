using InfoDynamics.Dominio.interfaces;
using InfoDynamics.Infraestructura.Contexto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfoDynamics.Infraestructura.Repositorio
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EmployeesDbContext _databasecontext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(EmployeesDbContext context)
        {
            _databasecontext = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
           public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
{
    return await _dbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(predicate);
}
        public async Task<List<T>> GetAllAsync(bool tracked = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);

            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
            }
        }

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            bool tracked = true,
            string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
                query = query.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public void SetOriginalConcurrencyToken(
            T entity,
            byte[] rowVersion,
            string tokenPropertyName = "RowVersion")
        {
            _databasecontext.Entry(entity).OriginalValues[tokenPropertyName] = rowVersion;
        }

     
    }
}