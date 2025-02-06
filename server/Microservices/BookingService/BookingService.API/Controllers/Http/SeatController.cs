using BookingService.Application.Handlers.Query.Seats.GetSeats;
using BookingService.Domain.Exceptions;

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

	[HttpGet("/AvailableSeats/Available/Session/{sessionId:Guid}")]
	public async Task<IActionResult> GetAvailable([FromRoute] Guid sessionId)
	{
		var seats = await _mediator.Send(new GetSeatsByIdQuery(sessionId, true))
			?? throw new NotFoundException(message: $"Available seats with session id '{sessionId}' not found.");

		return Ok(seats);
	}

	[HttpGet("/AvailableSeats/Reserved/Session/{sessionId:Guid}")]
	public async Task<IActionResult> GetReserved([FromRoute] Guid sessionId)
	{
		var seats = await _mediator.Send(new GetSeatsByIdQuery(sessionId, false))
			?? throw new NotFoundException(message: $"Reserved seats with session id '{sessionId}' not found.");

		return Ok(seats);
	}
}