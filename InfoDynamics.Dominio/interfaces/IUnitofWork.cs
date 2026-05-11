namespace InfoDynamics.Dominio.interfaces
{
    public interface IUnitOfWork
{
    Task SaveChangesAsync();
    IGenericRepository<T> Repository<T>() where T : class;
}}

