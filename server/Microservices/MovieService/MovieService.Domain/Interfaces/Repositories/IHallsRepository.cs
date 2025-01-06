using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IHallsRepository
{
	Task<Guid> CreateAsync(HallEntity hall, CancellationToken cancellationToken);
	void Delete(HallEntity hall);
	Task<HallEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<HallEntity>> GetAsync(CancellationToken cancellationToken);
	Task<HallEntity?> GetAsync(string name, CancellationToken cancellationToken);
	void Update(HallEntity hall, CancellationToken cancellationToken);
}