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

	public async Task<SessionEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Sessions
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<SessionEntity?> GetAsync(DateTime date, CancellationToken cancellationToken)
	{
		return await _context.Sessions
			.AsNoTracking()
			.Where(m => m.Date == date)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<SessionEntity>> GetAsync(CancellationToken cancellationToken)
	{
		var sessions = await _context.Sessions
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return sessions ?? [];
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

	public async Task<Guid> CreateAsync(SessionEntity session, CancellationToken cancellationToken)
	{
		await _context.Sessions.AddAsync(session, cancellationToken);

		return session.Id;
	}

	public void Update(SessionEntity session, CancellationToken cancellationToken)
	{
		_context.Sessions.Attach(session).State = EntityState.Modified;
	}

	public void Delete(SessionEntity session)
	{
		_context.Sessions.Attach(session);
		_context.Sessions.Remove(session);

		return;
	}
}