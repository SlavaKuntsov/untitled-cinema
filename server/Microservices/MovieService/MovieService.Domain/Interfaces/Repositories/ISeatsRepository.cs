using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Repositories;
public interface ISeatsRepository
{
	Task<SeatEntity?> GetAsync(int row, int column, CancellationToken cancellationToken);
	Task<SeatTypeEntity?> GetTypeAsync(string name, CancellationToken cancellationToken);
	Task<IList<SeatEntity>> GetBySessionIdAsync(Guid id, CancellationToken cancellationToken);
	Task CreateRangeAsync(IList<SeatEntity> seats, CancellationToken cancellationToken);
	void DeleteBySessionId(Guid id);
}
