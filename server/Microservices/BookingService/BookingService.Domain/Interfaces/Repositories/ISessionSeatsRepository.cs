using System.Linq.Expressions;

using BookingService.Domain.Entities;

namespace BookingService.Domain.Interfaces.Repositories;

public interface ISessionSeatsRepository
{
	Task<IList<SessionSeatsEntity>> GetAsync(CancellationToken cancellationToken);
	Task CreateAsync(SessionSeatsEntity seats, CancellationToken cancellationToken);
	Task<IList<SessionSeatsEntity>> GetAsync(Expression<Func<SessionSeatsEntity, bool>> predicate, bool isAvailableSeat, CancellationToken cancellationToken);
	Task<SessionSeatsEntity> GetAsync(Expression<Func<SessionSeatsEntity, bool>> predicate, CancellationToken cancellationToken);
	Task UpdateAsync(SessionSeatsEntity sessionSeats, CancellationToken cancellationToken);
}