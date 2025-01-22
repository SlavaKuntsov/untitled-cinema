using BookingService.Domain.Interfaces.Grpc;

using Protobufs.Auth;

namespace BookingService.Infrastructure.Grpc;

public class AuthGrpcService : IAuthGrpcService
{
	private readonly AuthService.AuthServiceClient _client;

	public AuthGrpcService(AuthService.AuthServiceClient client)
	{
		_client = client;
	}

	public async Task<bool> CheckExistAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var request = new CheckExistRequest()
		{
			UserId = id.ToString()
		};

		var response = await _client.CheckExistAsync(request, cancellationToken: cancellationToken);

		return response.IsExist;
	}
}