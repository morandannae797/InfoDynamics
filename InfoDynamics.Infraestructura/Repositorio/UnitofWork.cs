using InfoDynamics.Dominio.interfaces;
using InfoDynamics.Infraestructura.Contexto;
using Microsoft.EntityFrameworkCore;


namespace InfoDynamics.Infraestructura.Repositorio
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EmployeesDbContext _DbContext;
        private bool _disposed = false;

        public UnitOfWork(EmployeesDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task SaveChangesAsync()
        {
            await _DbContext.SaveChangesAsync();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
               return new GenericRepository<T>(_DbContext);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _DbContext.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}