using BookingService.Application.Handlers.Commands.Bookings.CreateBooking;
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

	[HttpGet("/Bookings")]
	public async Task<IActionResult> Get()
	{
		_logger.LogInformation("Fetch all bookings.");

		var bookings = await _mediator.Send(new GetAllBookingsQuery());

		_logger.LogInformation("Successfully fetched {Count} bookings.", bookings.Count);

		return Ok(bookings);
	}

	[HttpGet("/Bookings/{id:Guid}")]
	public async Task<IActionResult> Create([FromRoute] Guid id)
	{
		var bookings = await _mediator.Send(new GetUserBookingsQuery(id));

		return Ok(bookings);
	}

	[HttpPost("/Bookings")]
	public async Task<IActionResult> Create([FromBody] CreateBookingCommand request)
	{
		_logger.LogInformation($"Starting to create bookings {request.UserId} - {request.SessionId}.");

		var bookingId = await _mediator.Send(request);

		_logger.LogInformation($"Processed create bookings {request.UserId} - {request.SessionId}.");

		return Ok(bookingId);
	}

		return Ok("Your request is being processed");
	}
}