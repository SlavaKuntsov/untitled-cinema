using System.Linq.Expressions;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace BookingService.Persistence.Repositories;

public class BookingsRepository(BookingServiceDBContext context) : IBookingsRepository
{
	private readonly IMongoCollection<BookingEntity> _collection =
		context.GetCollection<BookingEntity>("Bookings");

	public async Task<IList<BookingEntity>> GetAsync(CancellationToken cancellationToken)
	{
		return await _collection
			.Find(FilterDefinition<BookingEntity>.Empty)
			.ToListAsync(cancellationToken);
	}

	public async Task<BookingEntity> GetOneAsync(
		Expression<Func<BookingEntity, bool>> predicate,
		CancellationToken cancellationToken)
	{
		return await _collection
			.Find(predicate)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<BookingEntity>> GetAsync(
		Expression<Func<BookingEntity, bool>> predicate,
		CancellationToken cancellationToken)
	{
		return await _collection
			.Find(predicate)
			.ToListAsync(cancellationToken);
	}

	public async Task CreateAsync(BookingEntity booking, CancellationToken cancellationToken)
	{
		var options = new InsertOneOptions();

		await _collection.InsertOneAsync(booking, options, cancellationToken);
	}

	public async Task UpdateStatusAsync(
		Guid bookingId,
		string status,
		CancellationToken cancellationToken)
	{
		var updateDefinition = Builders<BookingEntity>.Update
			.Set(x => x.Status, status);

		var options = new UpdateOptions();

		await _collection.UpdateOneAsync(
			b => b.Id == bookingId,
			updateDefinition,
			options,
			cancellationToken);
	}

	public async Task DeleteAsync(
		Expression<Func<BookingEntity, bool>> predicate,
		CancellationToken cancellationToken)
	{
		var options = new DeleteOptions();

		await _collection.DeleteOneAsync(
			predicate,
			options,
			cancellationToken);
	}
}