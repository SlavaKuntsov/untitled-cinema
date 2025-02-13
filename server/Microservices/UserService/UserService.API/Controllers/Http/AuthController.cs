using System.Security.Claims;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserService.Application.DTOs;
using UserService.Application.Handlers.Commands.Auth.Unauthorize;
using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Handlers.Queries.Tokens.GetByRefreshToken;
using UserService.Application.Handlers.Queries.Users.GetUserById;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Constants;
using UserService.Domain.Exceptions;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("/auth")]
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

	[HttpGet("refreshToken")]
	public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
	{
		var refreshToken = _cookieService.GetRefreshToken();

		var userRoleDto = await _mediator.Send(new GetByRefreshTokenCommand(refreshToken), cancellationToken);

		var authResultDto = await _mediator.Send(new GenerateTokensCommand(
			userRoleDto.Id,
			userRoleDto.Role),
			cancellationToken);

		HttpContext.Response.Cookies.Append(JwtConstants.REFRESH_COOKIE_NAME, authResultDto.RefreshToken);

		return Ok(new { authResultDto.AccessToken });
	}

	[HttpGet("authorize")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Authorize(CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		var user = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

		return Ok(_mapper.Map<UserDto>(user!));
	}

	[HttpGet("unauthorize")]
	[Authorize(Policy = "UserOrAdmin")]
	public IActionResult Unauthorize()
	{
		_cookieService.DeleteRefreshToken();

		return Ok();
	}
}