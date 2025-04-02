using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class HallsRepository(MovieServiceDBContext context) : IHallsRepository
{
	public async Task<HallEntity?> GetAsync(string name, CancellationToken cancellationToken)
	{
		return await context.Halls
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}
}