using MapsterMapper;

using MovieService.Domain.Interfaces.Grpc;

using Protobufs.Auth;

namespace MovieService.Infrastructure.Grpc;

public class AuthGrpcService : IAuthGrpcService
{
	private readonly AuthService.AuthServiceClient _client;
	private readonly IMapper _mapper;

	public AuthGrpcService(AuthService.AuthServiceClient client, IMapper mapper)
	{
		_client = client;
		_mapper = mapper;
	}

	public async Task<bool> CheckExistAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var request = _mapper.Map<CheckExistRequest>(id.ToString());

		var response = await _client.CheckExistAsync(request, cancellationToken: cancellationToken);

		return response.IsExist;
	}
}