using Grpc.Core;

using MapsterMapper;

using MediatR;

using Protobufs.Auth;

using UserService.Application.Handlers.Commands.Tokens.GenerateAccessToken;
using UserService.Application.Handlers.Commands.Tokens.RefreshToken;

namespace UserService.API.Controllers.Grpc.Auth;

public class AuthController : GetAccessTokenService.GetAccessTokenServiceBase
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
}