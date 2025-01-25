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

	public BookingController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/Bookings")]
	public async Task<IActionResult> Create()
	{
		var bookings = await _mediator.Send(new GetAllBookingsQuery());

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
		await _mediator.Send(request);

		return Ok("Your request is being processed");
	}
}