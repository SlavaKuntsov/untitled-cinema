using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Sessions;
using MovieService.API.Contracts.Requests;
using MovieService.Application.Handlers.Commands.Sessions.DeleteSession;
using MovieService.Application.Handlers.Commands.Sessions.FillSession;
using MovieService.Application.Handlers.Commands.Sessions.UpdateSession;
using MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;
using MovieService.Application.Handlers.Queries.Sessions.GetAllSessions;
using MovieService.Application.Handlers.Queries.Sessions.GetSessionById;

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

		var sessions = await _mediator.Send(new GetAllSessionsQuery(
			request.Limit,
			request.Offset,
			request.Movie,
			request.Date,
			request.Hall), cancellationToken);

		_logger.LogInformation("Successfully fetched {Count} movies.", request.Limit);

		return Ok(sessions);
	}

	[HttpGet("/sessions/{id:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		var sessions = await _mediator.Send(new GetSessionByIdQuery(id), cancellationToken);

		return Ok(sessions);
	}

	[HttpPost("/sessions")]
	[SwaggerRequestExample(typeof(FillSessionCommand), typeof(FillSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Fill([FromBody] FillSessionCommand request, CancellationToken cancellationToken)
	{
		var sessions = await _mediator.Send(request, cancellationToken);

		return Ok(sessions);
	}

	[HttpPatch("/sessions")]
	[SwaggerRequestExample(typeof(UpdateSessionCommand), typeof(UpdateSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateSessionCommand request, CancellationToken cancellationToken)
	{
		var sessions = await _mediator.Send(request, cancellationToken);

		return Ok(sessions);
	}

	[HttpDelete("/sessions/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		await _mediator.Send(new DeleteSessionCommand(id), cancellationToken);

		return NoContent();
	}

	[HttpGet("/sessions/seats/{sessionId:Guid}")]
	public async Task<IActionResult> GetSeats([FromRoute] Guid sessionId, CancellationToken cancellationToken)
	{
		var seats = await _mediator.Send(new GetSeatsBySessionIdQuery(sessionId), cancellationToken);

		return Ok(seats);
	}
}