using System.Security.Claims;

using BookingService.Application.Handlers.Commands.Bookings.CancelBooking;
using BookingService.Application.Handlers.Commands.Bookings.CreateBooking;
using BookingService.Application.Handlers.Commands.Bookings.PayBooking;
using BookingService.Application.Handlers.Commands.Seats.UpdateSeats;
using BookingService.Application.Handlers.Query.Bookings.GetAllBookings;
using BookingService.Application.Handlers.Query.Bookings.GetBookingsByUserId;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<BookingController> _logger;

	public BookingController(IMediator mediator, ILogger<BookingController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet("/bookings")]
	public async Task<IActionResult> Get()
	{
		_logger.LogInformation("Fetch all bookings.");

		var bookings = await _mediator.Send(new GetAllBookingsQuery());

		_logger.LogInformation("Successfully fetched {Count} bookings.", bookings.Count);

		return Ok(bookings);
	}

	[HttpGet("/bookings/{id:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid id)
	{
		var bookings = await _mediator.Send(new GetUserBookingsByIdQuery(id));

		return Ok(bookings);
	}

	[HttpPost("/bookings")]
	public async Task<IActionResult> Create([FromBody] CreateBookingCommand request)
	{
		//var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
		//	?? throw new UnauthorizedAccessException("User ID not found in claims.");

		//if (!Guid.TryParse(userIdClaim.Value, out var userId))
		//	throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		//var command = request with { UserId = userId };

		_logger.LogInformation("Starting to create bookings {UserId} - {SessionId}.",
			request.UserId,
			request.SessionId);

		var bookingId = await _mediator.Send(request);

		_logger.LogInformation("Processed create bookings {UserId} - {SessionId}.",
			request.UserId,
			request.SessionId);

		return Accepted();
		//return Ok(bookingId);
	}

	[HttpPatch("/bookings/pay/{bookingId:Guid}/user/{userId:Guid}")]
	public async Task<IActionResult> Pay([FromRoute] Guid bookingId, [FromRoute] Guid userId)
	{
		var booking = await _mediator.Send(new PayBookingCommand(bookingId, userId));

		return Ok(booking);
	}

	[HttpPatch("/bookings/cancel/{bookingId:Guid}")]
	public async Task<IActionResult> Cancel([FromRoute] Guid bookingId)
	{
		var booking = await _mediator.Send(new CancelBookingCommand(bookingId));

		await _mediator.Send(new UpdateSeatsCommand(
			booking.SessionId,
			booking.Seats,
			false));

		return Ok(booking);
	}
}