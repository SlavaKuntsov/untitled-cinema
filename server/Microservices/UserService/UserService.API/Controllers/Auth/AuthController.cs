using System.Diagnostics;
using System.Security.Claims;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using UserService.Application.Handlers.Commands.Tokens;
using UserService.Application.Handlers.Queries.Users;
using UserService.Domain.Exceptions;

namespace UserService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public AuthController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	// TODO - move to GRPC
	[HttpGet(nameof(RefreshToken))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> RefreshToken()
	{
		string? refreshToken = HttpContext.Request.Cookies[Domain.Constants.JwtConstants.COOKIE_NAME];

		if (string.IsNullOrEmpty(refreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var userRoleDto = await _mediator.Send(new RefreshTokenCommand(refreshToken));

		var authDto = await _mediator.Send(new GenerateAndUpdateTokensCommand(userRoleDto.Id, userRoleDto.Role));

		HttpContext.Response.Cookies.Append(Domain.Constants.JwtConstants.COOKIE_NAME, authDto.RefreshToken);

		return Ok(authDto);
	}

	[HttpGet(nameof(Authorize))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Authorize()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

		if (userIdClaim == null)
			throw new UnauthorizedAccessException("User ID not found in claims.");

		Guid userId = Guid.Parse(userIdClaim.Value);

		var user = await _mediator.Send(new GetUserQuery(userId))
			?? throw new NotFoundException("User not found");

		return Ok(_mapper.Map<UserDto>(user));
	}

	[HttpGet(nameof(Unauthorize))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult Unauthorize()
	{
		HttpContext.Response.Cookies.Delete(Domain.Constants.JwtConstants.COOKIE_NAME);

		return Ok();
	}
}