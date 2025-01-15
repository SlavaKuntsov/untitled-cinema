using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Persistence.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
	private readonly MovieServiceDBContext _context;

	public IDaysRepository DaysRepository { get; }
	public IHallsRepository HallsRepository { get; }
	public ISeatsRepository SeatsRepository { get; }
	public IMovieGenresRepository MovieGenresRepository { get; }
	public IMoviesRepository MoviesRepository { get; }
	public ISessionsRepository SessionsRepository { get; }

	public UnitOfWork(
		MovieServiceDBContext context,
		IDaysRepository daysRepository,
		IHallsRepository hallsRepository,
		ISeatsRepository seatsRepository,
		IMovieGenresRepository movieGenresRepository,
		IMoviesRepository moviesRepository,
		ISessionsRepository sessionsRepository)
	{
		_context = context;

		DaysRepository = daysRepository;
		HallsRepository = hallsRepository;
		SeatsRepository = seatsRepository;
		MovieGenresRepository = movieGenresRepository;
		MoviesRepository = moviesRepository;
		SessionsRepository = sessionsRepository;
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
