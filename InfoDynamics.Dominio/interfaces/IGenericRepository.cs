 using System.Linq.Expressions;
 namespace InfoDynamics.Dominio.interfaces
 {      
public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync(bool tracked = true);
        Task UpdateAsync(T entity);
        Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        bool tracked = true,
        string? includeProperties = null);
        Task DeleteByIdAsync(int id);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        void SetOriginalConcurrencyToken(T entity, byte[] rowVersion, string tokenPropertyName = "RowVersion");

    }
} 