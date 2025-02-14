using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class SessionsRepository : ISessionsRepository
{
	private readonly MovieServiceDBContext _context;

	public SessionsRepository(MovieServiceDBContext context)
	{
		_context = context;
	}

	public IQueryable<SessionEntity> Get()
	{
		return _context.Sessions.AsQueryable();
	}

	public async Task<List<SessionEntity>> ToListAsync(IQueryable<SessionEntity> query, CancellationToken cancellationToken)
	{
		return await query.ToListAsync(cancellationToken);
	}

	public async Task<IList<(Guid Id, Guid MovieId)>> GetOverlappingAsync(
		DateTime startTime,
		DateTime endTime,
		CancellationToken cancellationToken)
	{
		return await _context.Sessions
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