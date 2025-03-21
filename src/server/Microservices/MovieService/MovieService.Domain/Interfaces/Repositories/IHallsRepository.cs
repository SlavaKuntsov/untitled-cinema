using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IHallsRepository
{
	Task<HallEntity?> GetAsync(string name, CancellationToken cancellationToken);
}