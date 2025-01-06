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
			.Include(m => m.MovieGenres)
				.ThenInclude(mg => mg.Genre)
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public IQueryable<MovieEntity> Get()
	{
		return _context.Movies
			.AsNoTracking()
			.Include(m => m.MovieGenres)
				.ThenInclude(mg => mg.Genre)
			.AsQueryable();
	}

	public async Task<IList<MovieEntity>> ToListAsync(IQueryable<MovieEntity> query, CancellationToken cancellationToken)
	{
		return await query.ToListAsync(cancellationToken);
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

	public IQueryable<MovieEntity> FilterByGenre(IQueryable<MovieEntity> query, string genreFilter)
	{
		return query.Where(m => m.MovieGenres.Any(mg => mg.Genre.Name.ToLower() == genreFilter));
	}
}