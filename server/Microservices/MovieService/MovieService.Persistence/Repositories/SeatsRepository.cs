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

	public async Task<SeatEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Seats
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<SeatEntity?> GetAsync(int row, int column, CancellationToken cancellationToken)
	{
		return await _context.Seats
			.AsNoTracking()
			.Where(m => m.Row == row && m.Column == column)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<SeatEntity>> GetBySessionIdAsync(Guid id, CancellationToken cancellationToken)
	{
		var seats = await _context.Seats
			.AsNoTracking()
			.Include(s => s.Hall.Sessions
				.Where(ss => ss.Id == id))
			.ToListAsync(cancellationToken);

		return seats ?? [];
	}

	public async Task<SeatTypeEntity?> GetTypeAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.SeatTypes
			.AsNoTracking()
			.Where(m => m.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<SeatTypeEntity?> GetTypeAsync(string name, CancellationToken cancellationToken)
	{
		return await _context.SeatTypes
			.AsNoTracking()
			.Where(m => m.Name.ToLower() == name.ToLower())
			.FirstOrDefaultAsync(cancellationToken);
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

	public async Task<IList<SeatTypeEntity>> GetTypesAsync(CancellationToken cancellationToken)
	{
		var types = await _context.SeatTypes
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return types ?? [];
	}

	public async Task<Guid> CreateTypeAsync(SeatTypeEntity seatType, CancellationToken cancellationToken)
	{
		await _context.SeatTypes.AddAsync(seatType, cancellationToken);

		return seatType.Id;
	}

	public void Update(SeatEntity seat, CancellationToken cancellationToken)
	{
		_context.Seats.Attach(seat).State = EntityState.Modified;
	}

	public void Delete(SeatEntity seat)
	{
		_context.Seats.Attach(seat);
		_context.Seats.Remove(seat);

		return;
	}

	public void Update(SeatTypeEntity seatType, CancellationToken cancellationToken)
	{
		_context.SeatTypes.Attach(seatType).State = EntityState.Modified;
	}

	public void Delete(SeatTypeEntity seatType)
	{
		_context.SeatTypes.Attach(seatType);
		_context.SeatTypes.Remove(seatType);

		return;
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
}