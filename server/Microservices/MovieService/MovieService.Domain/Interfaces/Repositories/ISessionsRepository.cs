using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface ISessionsRepository
{
	Task<IList<SessionEntity>> GetOverlappingAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
	IQueryable<SessionEntity> Get();
	Task<List<SessionEntity>> ToListAsync(IQueryable<SessionEntity> query, CancellationToken cancellationToken);
}