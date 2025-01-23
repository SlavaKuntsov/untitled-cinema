using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface IDaysRepository
{
	Task<DayEntity?> GetAsync(DateTime date, CancellationToken cancellationToken);
}