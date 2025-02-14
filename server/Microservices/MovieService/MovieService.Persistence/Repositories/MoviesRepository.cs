using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;
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

	public async Task<int> GetCount()
	{
		return await _context.Movies
			.AsNoTracking()
			.CountAsync();
	}

	public async Task<IList<MovieEntity>> ToListAsync(IQueryable<MovieEntity> query, CancellationToken cancellationToken)
	{
		return await query
			.AsNoTracking().
			ToListAsync(cancellationToken);
	}

	public IQueryable<MovieEntity> FilterByGenre(IQueryable<MovieEntity> query, string genreFilter)
	{
		return query.Where(m => 
			m.MovieGenres.Any(mg => mg.Genre.Name.ToLower() == genreFilter));
	}

	public async Task<GenreEntity?> GetGenreByNameAsync(string name, CancellationToken cancellationToken)
	{
		return await _context.Genres
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}
}