using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IDaysRepository
{
	Task<Guid> CreateAsync(DayEntity day, CancellationToken cancellationToken);
	void Delete(DayEntity day);
	Task<DayEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<DayEntity?> GetAsync(DateTime date, CancellationToken cancellationToken);
	Task<IList<DayEntity>> GetAsync(CancellationToken cancellationToken);
	void Update(DayEntity day, CancellationToken cancellationToken);
}