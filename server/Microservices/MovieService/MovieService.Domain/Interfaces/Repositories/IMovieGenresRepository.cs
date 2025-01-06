using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IMovieGenresRepository
{
	Task AddAsync(GenreEntity genre, CancellationToken cancellationToken);
	Task<GenreEntity?> GetByNameAsync(string name, CancellationToken cancellationToken);
}