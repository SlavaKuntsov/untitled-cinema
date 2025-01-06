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

	public async Task<DayEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Days
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<DayEntity?> GetAsync(DateTime date, CancellationToken cancellationToken)
	{
		return await _context.Days
			.AsNoTracking()
			.Where(m => m.StartTime.Date == date)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<DayEntity>> GetAsync(CancellationToken cancellationToken)
	{
		var days = await _context.Days
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return days ?? [];
	}

	public async Task<Guid> CreateAsync(DayEntity day, CancellationToken cancellationToken)
	{
		await _context.Days.AddAsync(day, cancellationToken);

		return day.Id;
	}

	public void Update(DayEntity day, CancellationToken cancellationToken)
	{
		_context.Days.Attach(day).State = EntityState.Modified;
	}

	public void Delete(DayEntity day)
	{
		_context.Days.Attach(day);
		_context.Days.Remove(day);

		return;
	}
}