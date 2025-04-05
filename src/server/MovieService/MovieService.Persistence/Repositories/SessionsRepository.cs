using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class SessionsRepository(MovieServiceDBContext context) : ISessionsRepository
{
	public IQueryable<SessionEntity> Get()
	{
		return context.Sessions.AsQueryable();
	}
	
	public async Task<SessionEntity?> GetHallIdAsync(Guid sessionId, CancellationToken cancellationToken)
	{
		return await context.Sessions
			.AsNoTracking()
			.Where(s => s.Id == sessionId)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<List<SessionEntity>> ToListAsync(IQueryable<SessionEntity> query, CancellationToken cancellationToken)
	{
		return await query
			.Include(s => s.Hall)
			.OrderBy(s => s.StartTime)
			.ToListAsync(cancellationToken);
	}

	public async Task<IList<(Guid Id, Guid MovieId)>> GetOverlappingAsync(
		DateTime startTime,
		DateTime endTime,
		CancellationToken cancellationToken)
	{
		return await context.Sessions
			.AsNoTracking()
			.Where(s =>
				(s.StartTime < endTime && s.EndTime > startTime) ||
				(s.StartTime == startTime && s.EndTime == endTime)
			)
			.Select(s => new { s.Id, s.MovieId })
			.ToListAsync(cancellationToken)
			.ContinueWith(task => task.Result
				.Select(x => (x.Id, x.MovieId))
				.ToList());
	}
}