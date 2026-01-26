using System.Linq.Expressions;
using BookingService.Domain.Entities;

namespace BookingService.Domain.Interfaces.Repositories;

public interface IBookingsRepository
{
	IQueryable<BookingEntity> Get();
	Task<IList<BookingEntity>> GetAsync(CancellationToken cancellationToken);
	IQueryable<BookingEntity> Get(Expression<Func<BookingEntity, bool>> predicate);
	Task CreateAsync(BookingEntity booking, CancellationToken cancellationToken);
	Task<IList<BookingEntity>> GetAsync(
		Expression<Func<BookingEntity, bool>> predicate,
		CancellationToken cancellationToken);
	Task<BookingEntity?> GetOneAsync(
		Expression<Func<BookingEntity, bool>> predicate,
		CancellationToken cancellationToken);
	Task UpdateStatusAsync(Guid bookingId, string status, CancellationToken cancellationToken);
	Task DeleteAsync(Expression<Func<BookingEntity, bool>> predicate, CancellationToken cancellationToken);
	Task<IList<BookingEntity>> ToListAsync(
		IQueryable<BookingEntity> query,
		CancellationToken cancellationToken);
	Task<int> GetCount(IQueryable<BookingEntity> query);
}