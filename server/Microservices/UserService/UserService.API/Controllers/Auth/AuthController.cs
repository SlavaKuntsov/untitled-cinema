using MediatR;

using Microsoft.AspNetCore.Mvc;

using UserService.Application.Handlers.Commands.Tokens;

namespace UserService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;
	public AuthController(IMediator mediator)
	{
		_mediator = mediator;
	}

	// TODO - move to GRPC
	[HttpGet(nameof(RefreshToken))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> RefreshToken()
	{
		string? refreshToken = HttpContext.Request.Cookies[Application.Constants.JwtConstants.COOKIE_NAME];

		if (string.IsNullOrEmpty(refreshToken))
			return Unauthorized("Refresh token is missing.");

		var userDto = await _mediator.Send(new RefreshTokenCommand(refreshToken));

		var authResult = await _mediator.Send(new GenerateAndUpdateTokensCommand(userDto.Id, userDto.Role));

		HttpContext.Response.Cookies.Append(Application.Constants.JwtConstants.COOKIE_NAME, authResult.RefreshToken);

		return Ok(authResult);
	}

	// TODO - move to GRPC
	[HttpGet(nameof(Authorize))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Authorize()
	{
		string? refreshToken = HttpContext.Request.Cookies[Application.Constants.JwtConstants.COOKIE_NAME];

		if (string.IsNullOrEmpty(refreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var userDto = await _mediator.Send(new RefreshTokenCommand(refreshToken));

		var authResult = await _mediator.Send(new GenerateAndUpdateTokensCommand(userDto.Id, userDto.Role));

		HttpContext.Response.Cookies.Append(Application.Constants.JwtConstants.COOKIE_NAME, authResult.RefreshToken);

		return Ok(authResult);
	}

	// TODO - "Unauthorize" or "LogOut"?
	[HttpGet(nameof(Unauthorize))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult Unauthorize()
	{
		HttpContext.Response.Cookies.Delete(Application.Constants.JwtConstants.COOKIE_NAME);

		return Ok();
	}
}
