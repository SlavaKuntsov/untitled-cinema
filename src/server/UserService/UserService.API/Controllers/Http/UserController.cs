using System.Security.Claims;
using Domain.Constants;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using UserService.API.Contracts;
using UserService.API.Contracts.Examples;
using UserService.Application.DTOs;
using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Handlers.Commands.Users.ChangeBalance;
using UserService.Application.Handlers.Commands.Users.DeleteUser;
using UserService.Application.Handlers.Commands.Users.UpdateUser;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Application.Handlers.Queries.Users.GetAllUsers;
using UserService.Application.Handlers.Queries.Users.Login;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class UserController(IMediator mediator, IMapper mapper) : ControllerBase
{
	[HttpPost("/users/login")]
	[SwaggerRequestExample(typeof(CreateLoginRequest), typeof(CreateLoginRequestExample))]
	public async Task<IActionResult> Login(
		[FromBody] CreateLoginRequest request,
		CancellationToken cancellationToken)
	{
		var existUser = await mediator.Send(
			new LoginQuery(request.Email, request.Password),
			cancellationToken);

		var authResultDto = await mediator.Send(
			new GenerateTokensCommand(existUser.Id, existUser.Role),
			cancellationToken);

		HttpContext.Response.Cookies.Append(
			JwtConstants.REFRESH_COOKIE_NAME,
			authResultDto.RefreshToken);

		return Ok(
			new
			{
				accessToken = authResultDto.AccessToken,
				refreshToken = authResultDto.RefreshToken
			});
	}

	[HttpPost("/users/registration")]
	[SwaggerRequestExample(typeof(CreateUserRequest), typeof(CreateUserRequestExample))]
	public async Task<IActionResult> Registration(
		[FromBody] UserRegistrationCommand request,
		CancellationToken cancellationToken)
	{
		var authResultDto = await mediator.Send(request, cancellationToken);

		return Ok(new { authResultDto.AccessToken, authResultDto.RefreshToken });
	}

	[HttpPatch("/users")]
	[SwaggerRequestExample(typeof(UpdateUserRequest), typeof(UpdateUserRequestExample))]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Update(
		[FromBody] UpdateUserCommand request,
		CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
						?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		var command = request with { Id = userId };

		var user = await mediator.Send(command, cancellationToken);

		return Ok(mapper.Map<UserDto>(user));
	}

	[HttpPatch("/users/balance/increase/{amount:decimal}")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Balance(
		[FromRoute] decimal amount,
		CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
						?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		await mediator.Send(
			new ChangeBalanceCommand(userId, amount, true),
			cancellationToken);

		return Ok();
	}

	[HttpDelete("/users/{id:Guid?}")]
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
						?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		if (userId == id)
			throw new UnprocessableContentException("Admin cannot delete himself.");

		await mediator.Send(new DeleteUserCommand(id), cancellationToken);

		return Ok();
	}

	[HttpDelete("/users/me")]
	[Authorize(Policy = "UserOnly")]
	public async Task<IActionResult> Delete(CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
						?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		await mediator.Send(new DeleteUserCommand(userId), cancellationToken);

		return Ok();
	}

	[HttpGet("/users")]
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Users(CancellationToken cancellationToken)
	{
		var users = await mediator.Send(
			new GetAllUsersQuery(),
			cancellationToken);

		return Ok(mapper.Map<IList<UserDto>>(users));
	}
}