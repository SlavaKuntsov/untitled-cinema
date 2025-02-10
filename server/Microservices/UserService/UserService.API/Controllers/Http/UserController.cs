using System.Security.Claims;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Filters;

using UserService.API.Contracts;
using UserService.API.Contracts.Examples;
using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Handlers.Commands.Users.ChangeBalance;
using UserService.Application.Handlers.Commands.Users.DeleteUser;
using UserService.Application.Handlers.Commands.Users.UpdateUser;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Application.Handlers.Queries.Users.GetAllUsers;
using UserService.Application.Handlers.Queries.Users.GetUser;
using UserService.Application.Handlers.Queries.Users.Login;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public UserController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	[HttpPost("/users/login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[SwaggerRequestExample(typeof(CreateLoginRequest), typeof(CreateLoginRequestExample))]
	public async Task<IActionResult> Login([FromBody] CreateLoginRequest request)
	{
		var existUser = await _mediator.Send(new LoginQuery(request.Email, request.Password));

		var authResultDto = await _mediator.Send(new GenerateTokensCommand(existUser.Id, existUser.Role));

		HttpContext.Response.Cookies.Append(Domain.Constants.JwtConstants.COOKIE_NAME, authResultDto.RefreshToken);

		return Ok(authResultDto);
	}

	[HttpPost("/users/registration")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[SwaggerRequestExample(typeof(CreateUserRequest), typeof(CreateUserRequestExample))]
	public async Task<IActionResult> Registration([FromBody] UserRegistrationCommand request)
	{
		var authResultDto = await _mediator.Send(request);

		return Ok(authResultDto);
	}

	[HttpPatch("/users")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[SwaggerRequestExample(typeof(UpdateUserRequest), typeof(UpdateUserRequestExample))]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Update([FromBody] UpdateUserCommand request)
	{
		var particantModel = await _mediator.Send(request);

		return Ok(particantModel);
	}

	[HttpPatch("/users/balance/increase/{amount:decimal}")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Balance([FromRoute] decimal amount)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

		if (userIdClaim == null)
			throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		await _mediator.Send(new ChangeBalanceCommand(
			userId, 
			amount, 
			true));

		return Ok();
	}

	[HttpDelete("/users/{id:Guid}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

		if (userIdClaim == null)
			throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		if (userId != id)
			throw new UnauthorizedAccessException("User cannot delete another User.");

		await _mediator.Send(new DeleteUserCommand(id));
		return Ok();
	}

	[HttpGet("/users")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Users()
	{
		var users = await _mediator.Send(new GetAllUsersQuery());

		return Ok(_mapper.Map<IList<UserDto>>(users));
	}
}