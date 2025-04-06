using BookingService.Application.Handlers.Commands.Bookings.CancelBooking;
using BookingService.Application.Handlers.Commands.Bookings.CreateBooking;
using BookingService.Application.Handlers.Commands.Bookings.PayBooking;
using BookingService.Application.Handlers.Commands.Seats.UpdateSeats;
using BookingService.Application.Handlers.Query.Bookings.GetAllBookings;
using BookingService.Application.Handlers.Query.Bookings.GetUserBookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class BookingController(
	IMediator mediator,
	ILogger<BookingController> logger) : ControllerBase
{
	[HttpGet("/bookings")]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		logger.LogInformation("Fetch all bookings.");

		var bookings = await mediator.Send(new GetAllBookingsQuery(), cancellationToken);

		logger.LogInformation("Successfully fetched {Count} bookings.", bookings.Count);

		return Ok(bookings);
	}

	[HttpGet("/bookings/history/{userId:Guid}")]
	public async Task<IActionResult> Get(
		[FromRoute] Guid userId,
		CancellationToken cancellationToken,
		[FromQuery(Name = "OnlyProcessed")] bool processed = false)
	{
		var bookings = await mediator.Send(
			new GetUserBookingsByIdQuery(userId, processed),
			cancellationToken);

		return Ok(bookings);
	}

	[HttpPost("/bookings")]
	public async Task<IActionResult> Create(
		[FromBody] CreateBookingCommand request,
		CancellationToken cancellationToken)
	{
		//var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
		//	?? throw new UnauthorizedAccessException("User ID not found in claims.");

		//if (!Guid.TryParse(userIdClaim.Value, out var userId))
		//	throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		//var command = request with { UserId = userId };

		logger.LogInformation(
			"Starting to create bookings {UserId} - {SessionId}.",
			request.UserId,
			request.SessionId);

		var bookingId = await mediator.Send(request, cancellationToken);

		logger.LogInformation(
			"Processed create bookings {UserId} - {SessionId}.",
			request.UserId,
			request.SessionId);

		return Accepted();
		//return Ok(bookingId);
	}

	[HttpPatch("/bookings/pay/{bookingId:Guid}/user/{userId:Guid}")]
	public async Task<IActionResult> Pay(
		[FromRoute] Guid bookingId,
		[FromRoute] Guid userId,
		CancellationToken cancellationToken)
	{
		var booking = await mediator.Send(new PayBookingCommand(bookingId, userId), cancellationToken);

		return Ok(booking);
	}

	[HttpPatch("/bookings/cancel/{bookingId:Guid}")]
	public async Task<IActionResult> Cancel([FromRoute] Guid bookingId, CancellationToken cancellationToken)
	{
		var booking = await mediator.Send(new CancelBookingCommand(bookingId), cancellationToken);

		await mediator.Send(
			new UpdateSeatsCommand(
				booking.SessionId,
				booking.Seats,
				false),
			cancellationToken);

		return Ok(booking);
	}
}