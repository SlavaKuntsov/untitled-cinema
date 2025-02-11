using System.Security.Claims;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Handlers.Commands.Tokens.RefreshToken;
using UserService.Application.Handlers.Queries.Users.GetUser;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Constants;
using UserService.Domain.Exceptions;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ICookieService _cookieService;
	private readonly IMapper _mapper;

	public AuthController(IMediator mediator, ICookieService cookieService, IMapper mapper)
	{
		_mediator = mediator;
		_cookieService = cookieService;
		_mapper = mapper;
	}

	[HttpGet(nameof(RefreshToken))]
	public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
	{
		var refreshToken = _cookieService.GetRefreshToken();

		var userRoleDto = await _mediator.Send(new GetByRefreshTokenCommand(refreshToken), cancellationToken);

		var authResultDto = await _mediator.Send(new GenerateTokensCommand(
			userRoleDto.Id,
			userRoleDto.Role),
			cancellationToken);

		HttpContext.Response.Cookies.Append(JwtConstants.REFRESH_COOKIE_NAME, authResultDto.RefreshToken);

		return Ok(authResultDto.AccessToken);
	}

	[HttpGet(nameof(Authorize))]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Authorize(CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User ID not found in claims.");

		var userId = Guid.Parse(userIdClaim.Value);

		var user = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken)
			?? throw new NotFoundException("User not found");

		return Ok(_mapper.Map<UserDto>(user));
	}

	[HttpGet(nameof(Unauthorize))]
	[Authorize(Policy = "UserOrAdmin")]
	public IActionResult Unauthorize()
	{
		_cookieService.DeleteRefreshToken();

		return Ok();
	}
}