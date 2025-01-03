using Microsoft.EntityFrameworkCore;

using MovieService.Domain;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class MoviesRepository : IMoviesRepository
{
	private readonly MovieServiceDBContext _context;

	public MoviesRepository(MovieServiceDBContext context)
	{
		_context = context;
	}

	public async Task<MovieModel?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Movies
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<MovieModel>> GetAsync(CancellationToken cancellationToken)
	{
		var users = await _context.Movies
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return users ?? [];
	}

	public async Task<Guid> CreateAsync(MovieModel movie, CancellationToken cancellationToken)
	{
		await _context.Movies.AddAsync(movie, cancellationToken);

		return movie.Id;
	}

	public void Update(MovieModel movie, CancellationToken cancellationToken)
	{
		_context.Movies.Attach(movie).State = EntityState.Modified;
	}

	public void Delete(MovieModel movie)
	{
		_context.Movies.Attach(movie);
		_context.Movies.Remove(movie);

		return;
	}
}