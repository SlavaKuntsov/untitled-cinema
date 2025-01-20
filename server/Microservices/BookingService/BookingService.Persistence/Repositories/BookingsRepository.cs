using System.Linq.Expressions;

using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

using MongoDB.Driver;

namespace BookingService.Persistence.Repositories;

public class BookingsRepository : IBookingsRepository
{
	private readonly IMongoCollection<BookingEntity> _collection;

	public BookingsRepository(BookingServiceDBContext context)
	{
		_collection = context.GetCollection<BookingEntity>("Bookings");
	}

	public async Task<IList<BookingEntity>> GetAsync(CancellationToken cancellationToken)
	{
		return await _collection.Find(FilterDefinition<BookingEntity>.Empty).ToListAsync(cancellationToken);
	}

	public async Task<IList<BookingEntity>> GetAsync(
		Expression<Func<BookingEntity, bool>> predicate,
		CancellationToken cancellationToken)
	{
		return await _collection.Find(predicate).ToListAsync(cancellationToken);
	}

	public async Task CreateAsync(BookingEntity booking, CancellationToken cancellationToken)
	{
		var options = new InsertOneOptions();
		await _collection.InsertOneAsync(booking, options, cancellationToken);
	}
}