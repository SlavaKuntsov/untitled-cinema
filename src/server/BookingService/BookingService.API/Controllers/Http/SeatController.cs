using BookingService.Application.Handlers.Query.Seats.GetSeatsById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class SeatController(
	IMediator mediator,
	ILogger<SeatController> logger) : ControllerBase
{
	[HttpGet("/bookingsSeats/available/session/{sessionId:Guid}")]
	public async Task<IActionResult> GetAvailable(
		[FromRoute] Guid sessionId,
		CancellationToken cancellationToken)
	{
		var seats = await mediator.Send(new GetSeatsByIdQuery(sessionId, true), cancellationToken);

		return Ok(seats);
	}

	[HttpGet("/bookingsSeats/reserved/session/{sessionId:Guid}")]
	public async Task<IActionResult> GetReserved(
		[FromRoute] Guid sessionId,
		CancellationToken cancellationToken)
	{
		var seats = await mediator.Send(new GetSeatsByIdQuery(sessionId, false), cancellationToken);

		return Ok(seats);
	}
}