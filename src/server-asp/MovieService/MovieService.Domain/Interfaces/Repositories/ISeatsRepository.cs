using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;

public interface ISeatsRepository
{
	Task<SeatEntity?> GetAsync(Guid hallId, int row, int column, CancellationToken cancellationToken);
	Task<Guid?> GetIdAsync(Guid hallId, int row, int column, CancellationToken cancellationToken);
	Task<SeatTypeEntity?> GetTypeAsync(string name, CancellationToken cancellationToken);
	Task<SeatTypeEntity?> GetTypeAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<SeatEntity>> GetBySessionIdAsync(Guid id, CancellationToken cancellationToken);
	Task CreateRangeAsync(IList<SeatEntity> seats, CancellationToken cancellationToken);
	void DeleteBySessionId(Guid id);
	Task<IList<SeatTypeEntity>> GetTypeByHallIdAsync(Guid id, CancellationToken cancellationToken);
	void DeleteByHallId(Guid id);
}