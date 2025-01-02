using System.Security.Claims;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using UserService.Application.Handlers.Commands.Tokens.GenerateAccessToken;
using UserService.Application.Handlers.Commands.Tokens.RefreshToken;
using UserService.Application.Handlers.Queries.Users.GetUser;
using UserService.Application.Interfaces.Auth;
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
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> RefreshToken()
	{
		var refreshToken = _cookieService.GetRefreshToken();

		var userRoleDto = await _mediator.Send(new GetByRefreshTokenCommand(refreshToken));

		var accessToken = await _mediator.Send(new GenerateAccessTokenCommand(userRoleDto.Id, userRoleDto.Role));

		return Ok(accessToken);
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

		var userId = Guid.Parse(userIdClaim.Value);

		var user = await _mediator.Send(new GetUserByIdQuery(userId))
			?? throw new NotFoundException("User not found");

		return Ok(_mapper.Map<UserDto>(user));
	}

	[HttpGet(nameof(Unauthorize))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult Unauthorize()
	{
		_cookieService.DeleteRefreshToken();

		return Ok();
	}
}