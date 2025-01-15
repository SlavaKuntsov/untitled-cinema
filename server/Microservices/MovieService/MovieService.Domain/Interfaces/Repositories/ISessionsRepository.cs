using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface ISessionsRepository
{
	Task<Guid> CreateAsync(SessionEntity session, CancellationToken cancellationToken);
	void Delete(SessionEntity session);
	Task<SessionEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<SessionEntity>> GetAsync(CancellationToken cancellationToken);
	Task<IList<SessionEntity>> GetOverlappingAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
	void Update(SessionEntity session, CancellationToken cancellationToken);
	IQueryable<SessionEntity> Get();
	Task<List<SessionEntity>> ToListAsync(IQueryable<SessionEntity> query, CancellationToken cancellationToken);
}