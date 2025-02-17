using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Sessions;
using MovieService.API.Contracts.Requests;
using MovieService.Application.Handlers.Commands.Sessions.DeleteSession;
using MovieService.Application.Handlers.Commands.Sessions.FillSession;
using MovieService.Application.Handlers.Commands.Sessions.UpdateSession;
using MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;
using MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<MovieController> _logger;

	public SessionController(
		IMediator mediator,
		ILogger<MovieController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet("/sessions")]
	public async Task<IActionResult> Get([FromQuery] GetSessionsRequest request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Fetch all sessions.");

		var movies = await _mediator.Send(new GetAllSessionsQuery(
			request.Limit,
			request.Offset,
			request.Date,
			request.Hall), cancellationToken);

		_logger.LogInformation("Successfully fetched {Count} movies.", request.Limit);

		return Ok(movies);
	}

	[HttpPost("/sessions")]
	[SwaggerRequestExample(typeof(FillSessionCommand), typeof(FillSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Fill([FromBody] FillSessionCommand request, CancellationToken cancellationToken)
	{
		var movie = await _mediator.Send(request, cancellationToken);

		return Ok(movie);
	}

	[HttpPatch("/sessions")]
	[SwaggerRequestExample(typeof(UpdateSessionCommand), typeof(UpdateSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateSessionCommand request, CancellationToken cancellationToken)
	{
		var movie = await _mediator.Send(request, cancellationToken);

		return Ok(movie);
	}

	[HttpDelete("/sessions/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		await _mediator.Send(new DeleteSessionCommand(id), cancellationToken);

		return NoContent();
	}

	[HttpGet("/sessions/seats/{sessionId:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid sessionId, CancellationToken cancellationToken)
	{
		var seats = await _mediator.Send(new GetSeatsBySessionIdQuery(sessionId), cancellationToken);

		return Ok(seats);
	}
}