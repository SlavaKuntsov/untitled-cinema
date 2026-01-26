using System.Collections.Concurrent;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Persistence.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
	private readonly MovieServiceDBContext _context;
	private readonly ConcurrentDictionary<Type, object> _repositories = new();

	public IDaysRepository DaysRepository { get; }
	public IHallsRepository HallsRepository { get; }
	public ISeatsRepository SeatsRepository { get; }
	public IMoviesRepository MoviesRepository { get; }
	public ISessionsRepository SessionsRepository { get; }

	public UnitOfWork(
		MovieServiceDBContext context,
		IDaysRepository daysRepository,
		IHallsRepository hallsRepository,
		ISeatsRepository seatsRepository,
		IMoviesRepository moviesRepository,
		ISessionsRepository sessionsRepository)
	{
		_context = context;

		DaysRepository = daysRepository;
		HallsRepository = hallsRepository;
		SeatsRepository = seatsRepository;
		MoviesRepository = moviesRepository;
		SessionsRepository = sessionsRepository;
	}

	public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
	{
		var type = typeof(TEntity);
		if (!_repositories.ContainsKey(type))
		{
			var repositoryInstance = new GenericRepository<TEntity>(_context);
			_repositories[type] = repositoryInstance;
		}

		return (IGenericRepository<TEntity>)_repositories[type];
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken)
	{
		await _context.SaveChangesAsync(cancellationToken);
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}