using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class HallsRepository : IHallsRepository
{
	private readonly MovieServiceDBContext _context;

	public HallsRepository(MovieServiceDBContext context)
	{
		_context = context;
	}

	public async Task<HallEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Halls
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<HallEntity?> GetAsync(string name, CancellationToken cancellationToken)
	{
		return await _context.Halls
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<HallEntity>> GetAsync(CancellationToken cancellationToken)
	{
		var hallSeats = await _context.Halls
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return hallSeats ?? [];
	}

	public async Task<Guid> CreateAsync(HallEntity hall, CancellationToken cancellationToken)
	{
		await _context.Halls.AddAsync(hall, cancellationToken);

		return hall.Id;
	}

	public void Update(HallEntity hall, CancellationToken cancellationToken)
	{
		_context.Halls.Attach(hall).State = EntityState.Modified;
	}

	public void Delete(HallEntity hall)
	{
		_context.Halls.Attach(hall);
		_context.Halls.Remove(hall);

		return;
	}
}