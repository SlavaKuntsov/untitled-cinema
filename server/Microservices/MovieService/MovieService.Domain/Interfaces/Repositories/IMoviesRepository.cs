using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IMoviesRepository
{
	Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<MovieEntity>> GetAsync(CancellationToken cancellationToken);
	Task<Guid> CreateAsync(MovieEntity movie, CancellationToken cancellationToken);
	void Update(MovieEntity movie, CancellationToken cancellationToken);
	void Delete(MovieEntity movie);
}