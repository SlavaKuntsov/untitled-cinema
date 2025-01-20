using System.Linq.Expressions;

using BookingService.Domain.Entities;

namespace BookingService.Domain.Interfaces.Repositories;

public interface IBookingsRepository
{
	Task<IList<BookingEntity>> GetAsync(CancellationToken cancellationToken);
	Task CreateAsync(BookingEntity booking, CancellationToken cancellationToken);
	Task<IList<BookingEntity>> GetAsync(Expression<Func<BookingEntity, bool>> predicate, CancellationToken cancellationToken);
}