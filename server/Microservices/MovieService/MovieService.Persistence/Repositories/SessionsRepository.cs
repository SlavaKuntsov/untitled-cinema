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

	public async Task<IList<SessionEntity>> GetOverlappingAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
	{
		var sessions = await _context.Sessions
			.AsNoTracking()
			.Where(s =>
				(s.StartTime < endTime && s.EndTime > startTime) ||
				(s.StartTime == startTime && s.EndTime == endTime)
			)
			.ToListAsync(cancellationToken);

		return sessions ?? [];
	}
}