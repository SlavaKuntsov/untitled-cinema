using Grpc.Core;

using MapsterMapper;

using MediatR;

using Protobufs.Auth;

using UserService.Application.Handlers.Commands.Tokens.GenerateAccessToken;
using UserService.Application.Handlers.Commands.Tokens.RefreshToken;
using UserService.Application.Handlers.Queries.Users.GetUser;

namespace UserService.API.Controllers.Grpc.Auth;

public class AuthController : AuthService.AuthServiceBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public AuthController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	public override async Task<AccessTokenResponse> GetAccessToken(AccessTokenRequest request, ServerCallContext context)
	{
		var userRoleDto = await _mediator.Send(new GetByRefreshTokenCommand(request.RefreshToken));

		var accessToken = await _mediator.Send(new GenerateAccessTokenCommand(userRoleDto.Id, userRoleDto.Role));

		return _mapper.Map<AccessTokenResponse>(accessToken);
	}

	public override async Task<CheckExistResponse> CheckExist(CheckExistRequest request, ServerCallContext context)
	{
		var user = await _mediator.Send(new GetUserByIdQuery(Guid.Parse(request.UserId)));

		if (user is null)
			return _mapper.Map<CheckExistResponse>(false);

		return _mapper.Map<CheckExistResponse>(true);
	}
}