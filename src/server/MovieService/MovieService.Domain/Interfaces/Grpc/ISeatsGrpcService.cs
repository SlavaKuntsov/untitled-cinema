using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces.Grpc;

public interface ISeatsGrpcService
{
	public Task CreateEmptySessionSeats(
		Guid sessionId,
		IList<SeatEntity> availableSeats,
		CancellationToken cancellationToken = default);
}