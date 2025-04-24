using System.Security.Claims;
using Domain.Constants;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Contracts;
using UserService.Application.DTOs;
using UserService.Application.Handlers.Commands.Auth.Unauthorize;
using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Application.Handlers.Queries.Tokens.GetByRefreshToken;
using UserService.Application.Handlers.Queries.Users;
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

		return Ok(new { authResultDto.AccessToken, authResultDto.RefreshToken });
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


	[HttpGet("google-login")]
	public IActionResult GoogleLogin()
	{
		var redirectUrl = Url.Action("GoogleResponse", "Auth");
		var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

		return Challenge(properties, GoogleDefaults.AuthenticationScheme);
	}

	[HttpGet("google-response")]
	public async Task<IActionResult> GoogleResponse(CancellationToken cancellationToken)
	{
		var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

		if (!result.Succeeded)
			return BadRequest("Google authentication failed.");

		var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
		var firstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
		var lastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;

		if (string.IsNullOrEmpty(email))
			return BadRequest("Invalid Google credentials.");

		var user = await mediator.Send(new GetUserByEmailQuery(email), cancellationToken);

		var authResultDto = default(AuthDto);
		var text = default(string);

		if (user is not null)
		{
			authResultDto = await mediator.Send(
				new GenerateTokensCommand(user.Id, user.Role),
				cancellationToken);

			text = "login";
		}
		else
		{
			authResultDto = await mediator.Send(
				new UserRegistrationCommand(
					email,
					string.Empty,
					firstName,
					lastName,
					string.Empty),
				cancellationToken);

			text = "registration";
		}

		HttpContext.Response.Cookies.Append(
			JwtConstants.REFRESH_COOKIE_NAME,
			authResultDto.RefreshToken
		);

		return Ok(
			new
			{
				text,
				user,
				authResultDto
			});
	}

	[HttpPost("google-mobile-auth")]
	public async Task<IActionResult> GoogleMobileAuth(
		[FromBody] GoogleAuthRequest request,
		CancellationToken cancellationToken)
	{
		var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");

		var payload = await GoogleJsonWebSignature.ValidateAsync(
			request.IdToken,
			new GoogleJsonWebSignature.ValidationSettings
			{
				Audience = [clientId]
			});

		var email = payload.Email;
		var firstName = payload.GivenName;
		var lastName = payload.FamilyName;

		if (string.IsNullOrEmpty(email))
			return BadRequest("Invalid Google credentials.");

		var user = await mediator.Send(new GetUserByEmailQuery(email), cancellationToken);

		var authResultDto = default(AuthDto);
		var text = default(string);

		if (user is not null)
		{
			authResultDto = await mediator.Send(
				new GenerateTokensCommand(user.Id, user.Role),
				cancellationToken);

			text = "login";
		}
		else
		{
			authResultDto = await mediator.Send(
				new UserRegistrationCommand(
					email,
					string.Empty,
					firstName,
					lastName,
					string.Empty),
				cancellationToken);

			text = "registration";
		}

		HttpContext.Response.Cookies.Append(
			JwtConstants.REFRESH_COOKIE_NAME,
			authResultDto.RefreshToken
		);

		return Ok(
			new
			{
				text,
				user,
				authResultDto
			});
	}
}