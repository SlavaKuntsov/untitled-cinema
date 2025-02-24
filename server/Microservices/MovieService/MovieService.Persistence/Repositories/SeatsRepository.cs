using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class SeatsRepository : ISeatsRepository
{
	private readonly MovieServiceDBContext _context;

	public SeatsRepository(MovieServiceDBContext context)
	{
		_context = context;
	}

	public async Task<Guid?> GetAsync(int row, int column, CancellationToken cancellationToken)
	{
		return await _context.Seats
			.AsNoTracking()
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

		var seats = await _context.Seats
		.AsNoTracking()
		.Where(s => s.Hall.Sessions.Any(ss => ss.Id == id))
		.ToListAsync(cancellationToken);

		return seats;
	}

	public async Task<SeatTypeEntity?> GetTypeAsync(string name, CancellationToken cancellationToken)
	{
		return await _context.SeatTypes
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<SeatTypeEntity>> GetTypeByHallIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Seats
			.AsNoTracking()
			.Where(s => s.HallId == id)
			.Select(s => s.SeatType)
			.Distinct() 
			.ToListAsync(cancellationToken);
	}

	public async Task<Guid> CreateAsync(SeatEntity seat, CancellationToken cancellationToken)
	{
		await _context.Seats.AddAsync(seat, cancellationToken);

		return seat.Id;
	}

	public async Task CreateRangeAsync(IList<SeatEntity> seats, CancellationToken cancellationToken)
	{
		await _context.Seats.AddRangeAsync(seats, cancellationToken);
	}

	public void DeleteBySessionId(Guid id)
	{
		var seatsToDelete = _context.Seats
			.Include(s => s.Hall.Sessions.
				Where(ss => ss.Id == id))
			.ToList();

		_context.Seats.RemoveRange(seatsToDelete);

		return;
	}

	public void DeleteByHallId(Guid id)
	{
		var seatsToDelete = _context.Seats
			.Where(s => s.HallId == id)
			.ToList();

		_context.Seats.RemoveRange(seatsToDelete);

		return;
	}
}