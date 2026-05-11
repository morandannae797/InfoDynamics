using InfoDynamics.Dominio.interfaces;
using InfoDynamics.Infraestructura.Contexto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Polly.Retry;
using Polly;

namespace InfoDynamics.Infraestructura.Repositorio
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EmployeesDbContext _databasecontext;
        private readonly DbSet<T> _dbSet;
        private readonly AsyncRetryPolicy _asyncRetryPolicy;

        public GenericRepository(EmployeesDbContext context)
        {
            _databasecontext = context;
            _dbSet = context.Set<T>();
            _asyncRetryPolicy = Policy.Handle<DbUpdateConcurrencyException>().WaitAndRetryAsync
                (3 ,retryAttempt=> TimeSpan.FromSeconds
            (Math.Pow(2,retryAttempt)));
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T?> GetByIdAsync(int id)  
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync(bool tracked = true)  
        {
            IQueryable<T> query = _dbSet;
            if (!tracked) query = query.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await _asyncRetryPolicy.ExecuteAsync(async ()=> {


                try
                {
                    _dbSet.Update(entity);
                    await SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                        var entry = ex.Entries.Single();
   
    var bdvalor = await entry.GetDatabaseValuesAsync();
    var clientevalor = entry.CurrentValues.Clone();
    await entry.ReloadAsync();
    entry.CurrentValues.SetValues(clientevalor);
                }

            });
            

            
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _databasecontext.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            bool tracked = true,
            string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked) query = query.AsNoTracking();
            if (filter != null) query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            bool tracked = true,
            string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked) query = query.AsNoTracking();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync(filter);
        }

 
        public Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        public void SetOriginalConcurrencyToken(T entity, byte[] rowVersion, string tokenPropertyName = "RowVersion")
        {
            // Entity Framework requiere establecer el valor original para lanzar la DbUpdateConcurrencyException
            _databasecontext.Entry(entity).OriginalValues[tokenPropertyName] = rowVersion;
        }
    }
}