using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface ISessionsRepository
{
	IQueryable<SessionEntity> Get();
	Task<List<SessionEntity>> ToListAsync(IQueryable<SessionEntity> query, CancellationToken cancellationToken);
	Task<IList<(Guid Id, Guid MovieId)>> GetOverlappingAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}