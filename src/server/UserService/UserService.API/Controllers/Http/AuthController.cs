using System.Security.Claims;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Handlers.Commands.Auth.Unauthorize;
using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Handlers.Queries.Tokens.GetByRefreshToken;
using UserService.Application.Handlers.Queries.Users.GetUserById;
using Utilities.Service;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("/auth")]
public class AuthController(IMediator mediator, ICookieService cookieService) : ControllerBase
{
	[HttpGet("refreshToken")]
	public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
	{
		var refreshToken = cookieService.GetRefreshToken();

		var userRoleDto = await mediator.Send(
			new GetByRefreshTokenCommand(
				refreshToken),
			cancellationToken);

		var authResultDto = await mediator.Send(
			new GenerateTokensCommand(userRoleDto.Id, userRoleDto.Role),
			cancellationToken);

		HttpContext.Response.Cookies.Append(
			JwtConstants.REFRESH_COOKIE_NAME,
			authResultDto.RefreshToken);

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

		var user = await mediator.Send(
			new GetUserByIdQuery(userId),
			cancellationToken);

		return Ok(user);
	}

	[HttpGet("unauthorize")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Unauthorize(CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
						?? throw new UnauthorizedAccessException("User ID not found in claims.");

		var userId = Guid.Parse(userIdClaim.Value);

		cookieService.DeleteRefreshToken();

		await mediator.Send(new UnauthorizeCommand(userId), cancellationToken);

		return Ok();
	}
}