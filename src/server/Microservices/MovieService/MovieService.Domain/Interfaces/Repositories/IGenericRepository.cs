namespace MovieService.Domain.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
	Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<T>> GetAsync(CancellationToken cancellationToken);
	Task CreateAsync(T entity, CancellationToken cancellationToken);
	void Update(T entity);
	void Delete(T entity);
}