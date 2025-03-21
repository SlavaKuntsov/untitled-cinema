using BookingService.Application.Handlers.Query.Seats.GetSeatsById;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class SeatController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<SeatController> _logger;

	public SeatController(IMediator mediator, ILogger<SeatController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet("/bookingsSeats/available/session/{sessionId:Guid}")]
	public async Task<IActionResult> GetAvailable([FromRoute] Guid sessionId)
	{
		var seats = await _mediator.Send(new GetSeatsByIdQuery(sessionId, true));

		return Ok(seats);
	}

	[HttpGet("/bookingsSeats/reserved/session/{sessionId:Guid}")]
	public async Task<IActionResult> GetReserved([FromRoute] Guid sessionId)
	{
		var seats = await _mediator.Send(new GetSeatsByIdQuery(sessionId, false));

		return Ok(seats);
	}
}