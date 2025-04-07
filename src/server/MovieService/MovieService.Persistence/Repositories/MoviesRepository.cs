using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class MoviesRepository(MovieServiceDBContext context) : IMoviesRepository
{
	public async Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.Movies
			.AsNoTracking()
			.Include(m => m.MovieGenres)
				.ThenInclude(mg => mg.Genre)
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public IQueryable<MovieEntity> Get()
	{
		return context.Movies
			.AsNoTracking()
			.AsQueryable();
	}

	public IQueryable<MovieEntity> Get(DateTime date)
	{
		var parsedDate = date.Date;

		var days = context.Days
			.Where(d => d.StartTime.Date == parsedDate || d.EndTime.Date == parsedDate);

		var movieIds = context.Sessions
			.Where(s => days.Contains(s.Day) &&
						(s.StartTime.Date == parsedDate || s.EndTime.Date == parsedDate))
			.Select(s => s.MovieId)
			.Distinct();

		var movies = context.Movies
			.Where(m => movieIds.Contains(m.Id));

		return movies;
	}

	public async Task<int> GetCount(IQueryable<MovieEntity> query)
	{
		return await query.AsNoTracking().CountAsync();
	}

	public async Task<IList<MovieEntity>> ToListAsync(IQueryable<MovieEntity> query, CancellationToken cancellationToken)
	{
		return await query
			.AsNoTracking()
			.Include(m => m.MovieGenres)
				.ThenInclude(mg => mg.Genre)
			.ToListAsync(cancellationToken);
	}

	public IQueryable<MovieEntity> FilterByGenre(IQueryable<MovieEntity> query, List<string> genreFilters)
	{
		return query.Where(m =>
			m.MovieGenres.Any(mg =>
				genreFilters.Contains(mg.Genre.Name.ToLower())));
	}

	public async Task<GenreEntity?> GetGenreByNameAsync(string name, CancellationToken cancellationToken)
	{
		return await context.Genres
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}
}