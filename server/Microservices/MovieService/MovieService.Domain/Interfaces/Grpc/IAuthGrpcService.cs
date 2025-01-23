
namespace MovieService.Domain.Interfaces.Grpc;

public interface IAuthGrpcService
{
	Task<bool> CheckExistAsync(Guid id, CancellationToken cancellationToken);
}