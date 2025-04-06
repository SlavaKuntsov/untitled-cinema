using Grpc.Core;
using MapsterMapper;
using MediatR;
using Protobufs.Auth;
using UserService.Application.Handlers.Queries.Tokens.GetByRefreshToken;
using UserService.Application.Handlers.Queries.Users.GetUserExist;
using UserService.Application.Interfaces.Auth;

namespace UserService.API.Controllers.Grpc;

public class AuthController(
	IMediator mediator,
	IJwt jwt,
	IMapper mapper) : AuthService.AuthServiceBase
{
	public override async Task<AccessTokenResponse> GetAccessToken(
		AccessTokenRequest request,
		ServerCallContext context)
	{
		var userRoleDto = await mediator.Send(new GetByRefreshTokenCommand(request.RefreshToken));

		var accessToken = jwt.GenerateAccessToken(userRoleDto.Id, userRoleDto.Role);

		return mapper.Map<AccessTokenResponse>(accessToken);
	}

	public override async Task<CheckExistResponse> CheckExist(
		CheckExistRequest request,
		ServerCallContext context)
	{
		var user = await mediator.Send(new GetUserExistQuery(Guid.Parse(request.UserId)));

		return new CheckExistResponse
		{
			IsExist = user
		};
	}
}