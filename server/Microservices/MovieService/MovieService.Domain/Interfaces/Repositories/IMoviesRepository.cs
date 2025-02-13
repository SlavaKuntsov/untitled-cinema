using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IMoviesRepository
{
	Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	IQueryable<MovieEntity> Get();
	Task<int> GetCount();
	Task<IList<MovieEntity>> ToListAsync(IQueryable<MovieEntity> query, CancellationToken cancellationToken);
	IQueryable<MovieEntity> FilterByGenre(IQueryable<MovieEntity> query, string genreFilter);
	Task<GenreEntity?> GetGenreByNameAsync(string name, CancellationToken cancellationToken);
}