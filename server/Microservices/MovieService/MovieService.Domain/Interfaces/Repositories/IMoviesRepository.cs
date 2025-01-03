

namespace MovieService.Domain.Interfaces.Repositories;

public interface IMoviesRepository
{
	Task<MovieModel?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<MovieModel>> GetAsync(CancellationToken cancellationToken);
	Task<Guid> CreateAsync(MovieModel movie, CancellationToken cancellationToken);
	void Update(MovieModel movie, CancellationToken cancellationToken);
	void Delete(MovieModel movie);
}