using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;
public interface ISeatsRepository
{
	Task<SeatEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<SeatEntity?> GetAsync(int row, int column, CancellationToken cancellationToken);
	Task<SeatTypeEntity?> GetTypeAsync(string name, CancellationToken cancellationToken);
	Task<SeatTypeEntity?> GetTypeAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<SeatEntity>> GetBySessionIdAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<SeatTypeEntity>> GetTypesAsync(CancellationToken cancellationToken);
	Task<Guid> CreateAsync(SeatEntity seat, CancellationToken cancellationToken);
	Task<Guid> CreateTypeAsync(SeatTypeEntity seatType, CancellationToken cancellationToken);
	void Update(SeatEntity seat, CancellationToken cancellationToken);
	void Delete(SeatEntity seat);
	void Update(SeatTypeEntity seatType, CancellationToken cancellationToken);
	void Delete(SeatTypeEntity seatType);
	Task CreateRangeAsync(IList<SeatEntity> seats, CancellationToken cancellationToken);
	void DeleteBySessionId(Guid id);
}
