using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IMoviesRepository
{
	Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	IQueryable<MovieEntity> Get();
	Task<IList<MovieEntity>> ToListAsync(IQueryable<MovieEntity> query, CancellationToken cancellationToken);
	IQueryable<MovieEntity> FilterByGenre(IQueryable<MovieEntity> query, string genreFilter);
	Task<GenreEntity?> GetGenreByNameAsync(string name, CancellationToken cancellationToken);
	Task<int> GetCount(IQueryable<MovieEntity> query);
}