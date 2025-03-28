using System.Linq.Expressions;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace BookingService.Persistence.Repositories;

public class SessionSeatsRepository(BookingServiceDBContext context) : ISessionSeatsRepository
{
	private readonly IMongoCollection<SessionSeatsEntity> _collection =
		context.GetCollection<SessionSeatsEntity>("Seats");

	public async Task<IList<SessionSeatsEntity>> GetAsync(CancellationToken cancellationToken)
	{
		return await _collection.Find(FilterDefinition<SessionSeatsEntity>.Empty)
			.ToListAsync(cancellationToken);
	}

	public async Task<SessionSeatsEntity?> GetAsync(
		Expression<Func<SessionSeatsEntity, bool>> predicate,
		bool isAvailableSeats,
		CancellationToken cancellationToken)
	{
		var entities = await _collection
			.Find(predicate)
			.ToListAsync(cancellationToken);

		return entities.Select(
				entity => new SessionSeatsEntity
				{
					Id = entity.Id,
					SessionId = entity.SessionId,
					AvailableSeats = isAvailableSeats ? entity.AvailableSeats : null,
					ReservedSeats = !isAvailableSeats ? entity.ReservedSeats : null,
					UpdatedAt = entity.UpdatedAt
				})
			.FirstOrDefault();
	}

	public async Task<SessionSeatsEntity> GetAsync(
		Expression<Func<SessionSeatsEntity, bool>> predicate,
		CancellationToken cancellationToken)
	{
		return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
	}

	public async Task CreateAsync(SessionSeatsEntity sessionSeats, CancellationToken cancellationToken)
	{
		var options = new InsertOneOptions();
		await _collection.InsertOneAsync(sessionSeats, options, cancellationToken);
	}

	public async Task UpdateAsync(SessionSeatsEntity sessionSeats, CancellationToken cancellationToken)
	{
		var options = new ReplaceOptions();

		await _collection.ReplaceOneAsync(
			x => x.Id == sessionSeats.Id,
			sessionSeats,
			options,
			cancellationToken);
	}
}