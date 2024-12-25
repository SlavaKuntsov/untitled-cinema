using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserService.API.Contracts;
using UserService.Application.Handlers.Commands.Tokens;
using UserService.Application.Handlers.Commands.Users;
using UserService.Application.Handlers.Queries.Users;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;

namespace UserService.API.Controllers.Users;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private readonly IMediator _mediator;
	public UserController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost(nameof(Login))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Login([FromBody] CreateLoginRequest request)
	{
		var existUser = await _mediator.Send(new LoginQuery(request.Email, request.Password));

		var authResultDto = await _mediator.Send(new GenerateAndUpdateTokensCommand(existUser.Id, existUser.Role));

		HttpContext.Response.Cookies.Append(Application.Constants.JwtConstants.COOKIE_NAME, authResultDto.RefreshToken);

		return Ok(authResultDto);
	}

	[HttpPost(nameof(Registration))]
	[ProducesResponseType(StatusCodes.Status200OK)]

	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Registration([FromBody] CreateUserRequest request)
	{
		if (!Enum.TryParse<Role>(request.Role, out var role))
			throw new BadRequestException("Such role does not exist");

		if (role != Role.User)
			throw new BadRequestException("Role does not equal the necessary one");

		var authResult = await _mediator.Send(new UserRegistrationCommand(
			request.Email,
			request.Password,
			role,
			request.Firstname,
			request.Lastname,
			request.DateOfBirth));

		return Ok(authResult);
	}

	[HttpPut(nameof(Update))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
	{
		var particantModel =  await _mediator.Send(new UpdateUserCommand(
			request.Id,
			request.Firstname,
			request.Lastname,
			request.DateOfBirth));

		return Ok(particantModel);
	}

	[HttpDelete(nameof(Delete) + "/{id:Guid}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteUserCommand(id));
		return Ok();
	}

	[HttpGet(nameof(Users))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Users()
	{
		var users = await _mediator.Send(new GetAllUsersQuery());

		return Ok(users);
	}
}