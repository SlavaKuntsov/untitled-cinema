// using MovieService.Domain.Interfaces.Grpc;
//
// using Protobufs.Auth;
//
// namespace MovieService.Infrastructure.Grpc;
//
// public class AuthGrpcService(AuthService.AuthServiceClient client) : IAuthGrpcService
// {
// 	public async Task<bool> CheckExistAsync(Guid id, CancellationToken cancellationToken = default)
// 	{
// 		var request = new CheckExistRequest()
// 		{
// 			UserId = id.ToString()
// 		};
//
// 		var response = await client.CheckExistAsync(request, cancellationToken: cancellationToken);
//
// 		return response.IsExist;
// 	}
// }