using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
	private readonly MovieServiceDBContext _context;
	private readonly DbSet<T> _dbSet;

	public GenericRepository(MovieServiceDBContext context)
	{
		_context = context;
		_dbSet = _context.Set<T>();
	}

	public async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _dbSet.FindAsync(id, cancellationToken);
	}

	public async Task<IList<T>> GetAsync(CancellationToken cancellationToken)
	{
		return await _dbSet
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task CreateAsync(T entity, CancellationToken cancellationToken)
	{
		await _dbSet.AddAsync(entity, cancellationToken);
	}

	public void Update(T entity)
	{
		_dbSet.Attach(entity);
		_context.Entry(entity).State = EntityState.Modified;
	}

	public void Delete(T entity)
	{
		_dbSet.Attach(entity);
		_dbSet.Remove(entity);
	}
}