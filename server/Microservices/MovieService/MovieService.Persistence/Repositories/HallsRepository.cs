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

	public async Task<HallEntity?> GetAsync(string name, CancellationToken cancellationToken)
	{
		return await _context.Halls
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}
}