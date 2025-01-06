using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class MoviesRepository : IMoviesRepository
{
	private readonly MovieServiceDBContext _context;

	public MoviesRepository(MovieServiceDBContext context)
	{
		_context = context;
	}

	public async Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Movies
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<MovieEntity>> GetAsync(CancellationToken cancellationToken)
	{
		var movies = await _context.Movies
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return movies ?? [];
	}

	public async Task<Guid> CreateAsync(MovieEntity movie, CancellationToken cancellationToken)
	{
		await _context.Movies.AddAsync(movie, cancellationToken);

		return movie.Id;
	}

	public void Update(MovieEntity movie, CancellationToken cancellationToken)
	{
		_context.Movies.Attach(movie).State = EntityState.Modified;
	}

	public void Delete(MovieEntity movie)
	{
		_context.Movies.Attach(movie);
		_context.Movies.Remove(movie);

		return;
	}
}