using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Sessions;
using MovieService.Application.Handlers.Commands.Sessions.DeleteSession;
using MovieService.Application.Handlers.Commands.Sessions.UpdateSession;
using MovieService.Application.Handlers.Commands.Sessoins.FillSession;
using MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;
using MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
	private readonly IMediator _mediator;

	public SessionController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/Sessions")]
	public async Task<IActionResult> Get(
		[FromQuery] byte limit = 10,
		[FromQuery] byte offset = 1,
		[FromQuery] string? date = null,
		[FromQuery] string? hall = null)
	{
		var movies = await _mediator.Send(new GetAllSessionsQuery(
			limit,
			offset,
			date,
			hall));

		return Ok(movies);
	}

	[HttpPost("/Sessions")]
	[SwaggerRequestExample(typeof(FillSessionCommand), typeof(FillSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Fill([FromBody] FillSessionCommand request)
	{
		// TODO - maybe create session by all name instead of hall id
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch("/Sessions")]
	[SwaggerRequestExample(typeof(UpdateSessionCommand), typeof(UpdateSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateSessionCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete("/Sessions/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteSessionCommand(id));

		return NoContent();
	}

	[HttpGet("/Sessions/Seats/{sessionId:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid sessionId)
	{
		var seats = await _mediator.Send(new GetSeatsBySessionIdQuery(sessionId));

		return Ok(seats);
	}
}