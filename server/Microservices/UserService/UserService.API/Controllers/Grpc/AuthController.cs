using Grpc.Core;

using MapsterMapper;

using MediatR;

using Protobufs.Auth;

using UserService.Application.Handlers.Commands.Tokens;

namespace UserService.API.Controllers.Grpc.Auth;

public class AuthController : RefreshTokenService.RefreshTokenServiceBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public AuthController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
	{
		if (string.IsNullOrEmpty(request.RefreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var userRoleDto = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));

		var authDto = await _mediator.Send(new GenerateAndUpdateTokensCommand(userRoleDto.Id, userRoleDto.Role));

		return _mapper.Map<RefreshTokenResponse>(authDto);
	}
}