using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class SeatsRepository(MovieServiceDBContext context) : ISeatsRepository
{
	public async Task<SeatEntity?> GetAsync(Guid hallId, int row, int column, CancellationToken cancellationToken)
	{
		return await context.Seats
			.AsNoTracking()
			.Where(s => s.HallId == hallId)
			.Where(m => m.Row == row && m.Column == column)
			.FirstOrDefaultAsync(cancellationToken);
	}
	
	public async Task<Guid?> GetIdAsync(Guid hallId, int row, int column, CancellationToken cancellationToken)
	{
		return await context.Seats
			.AsNoTracking()
			.Where(s => s.HallId == hallId)
			.Where(m => m.Row == row && m.Column == column)
			.Select(s => s.Id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<SeatEntity>> GetBySessionIdAsync(Guid id, CancellationToken cancellationToken)
	{
		//var seats = await _context.Seats
		//	.AsNoTracking()
		//	.Include(s => s.Hall.Sessions
		//		.Where(ss => ss.Id == id))
		//	.ToListAsync(cancellationToken);

		//return seats ?? [];

		var seats = await context.Seats
			.AsNoTracking()
			.Where(s => s.Hall.Sessions.Any(ss => ss.Id == id))
			.ToListAsync(cancellationToken);

		return seats;
	}

	public async Task<SeatTypeEntity?> GetTypeAsync(string name, CancellationToken cancellationToken)
	{
		return await context.SeatTypes
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}
	public async Task<SeatTypeEntity?> GetTypeAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.SeatTypes
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<SeatTypeEntity>> GetTypeByHallIdAsync(
		Guid id,
		CancellationToken cancellationToken)
	{
		return await context.Seats
			.AsNoTracking()
			.Where(s => s.HallId == id)
			.Select(s => s.SeatType)
			.Distinct()
			.ToListAsync(cancellationToken);
	}

	public async Task CreateRangeAsync(IList<SeatEntity> seats, CancellationToken cancellationToken)
	{
		await context.Seats.AddRangeAsync(seats, cancellationToken);
	}

	public void DeleteBySessionId(Guid id)
	{
		var seatsToDelete = context.Seats
			.Include(s => s.Hall.Sessions.Where(ss => ss.Id == id))
			.ToList();

		context.Seats.RemoveRange(seatsToDelete);
	}

	public void DeleteByHallId(Guid id)
	{
		var seatsToDelete = context.Seats
			.Where(s => s.HallId == id)
			.ToList();

		context.Seats.RemoveRange(seatsToDelete);
	}

	public async Task<Guid> CreateAsync(SeatEntity seat, CancellationToken cancellationToken)
	{
		await context.Seats.AddAsync(seat, cancellationToken);

		return seat.Id;
	}
}