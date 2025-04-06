using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class DaysRepository(MovieServiceDBContext context) : IDaysRepository
{
	public async Task<DayEntity?> GetAsync(DateTime date, CancellationToken cancellationToken)
	{
		return await context.Days
			.AsNoTracking()
			.Where(m => m.StartTime.Date == date)
			.FirstOrDefaultAsync(cancellationToken);
	}
}