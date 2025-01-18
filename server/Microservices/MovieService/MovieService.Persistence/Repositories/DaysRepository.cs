using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class DaysRepository : IDaysRepository
{
	private readonly MovieServiceDBContext _context;

	public DaysRepository(MovieServiceDBContext context)
	{
		_context = context;
	}
	public async Task<DayEntity?> GetAsync(DateTime date, CancellationToken cancellationToken)
	{
		return await _context.Days
			.AsNoTracking()
			.Where(m => m.StartTime.Date == date)
			.FirstOrDefaultAsync(cancellationToken);
	}
}