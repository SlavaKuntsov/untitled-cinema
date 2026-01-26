using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IMoviesRepository
{
	Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	IQueryable<MovieEntity> Get();
	IQueryable<MovieEntity> Get(DateTime date);
	Task<IList<MovieEntity>> ToListAsync(IQueryable<MovieEntity> query, CancellationToken cancellationToken);
	Task<GenreEntity?> GetGenreByNameAsync(string name, CancellationToken cancellationToken);
	Task<int> GetCount(IQueryable<MovieEntity> query);
	IQueryable<MovieEntity> FilterByGenre(IQueryable<MovieEntity> query, List<string> genreFilters);
	Task<IList<MovieFrameEntity>> GetFramesAsync(Guid movieId, CancellationToken cancellationToken);
}