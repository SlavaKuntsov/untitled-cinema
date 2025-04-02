using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieService.API.Contracts.RequestExamples.Seats;
using MovieService.Application.Handlers.Commands.Seats.CreateSeat;
using MovieService.Application.Handlers.Commands.Seats.CreateSeatType;
using MovieService.Application.Handlers.Commands.Seats.DeleteSeatType;
using MovieService.Application.Handlers.Commands.Seats.UpdateSeat;
using MovieService.Application.Handlers.Commands.Seats.UpdateSeatType;
using MovieService.Application.Handlers.Queries.Seats.GetAllSeatTypes;
using MovieService.Application.Handlers.Queries.Seats.GetSeatById;
using MovieService.Application.Handlers.Queries.Seats.GetSeatByIndex;
using MovieService.Application.Handlers.Queries.Seats.GetSeatTypesByHallId;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
//[Authorize(Policy = "AdminOnly")]
public class SeatController(IMediator mediator) : ControllerBase
{
	[HttpGet("/seats/{id:Guid}")]
	public async Task<IActionResult> Get(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		var seat = await mediator.Send(new GetSeatByIdQuery(id), cancellationToken);

		return Ok(seat);
	}

	[HttpGet("/seats/{hallId:Guid}/{row:int}/{column:int}")]
	public async Task<IActionResult> Get(
		[FromRoute] Guid hallId,
		[FromRoute] int row,
		[FromRoute] int column,
		CancellationToken cancellationToken)
	{
		var seat = await mediator.Send(
			new GetSeatByIndexQuery(
				hallId,
				row,
				column),
			cancellationToken);

		return Ok(seat);
	}

	[HttpPost("/seats")]
	[SwaggerRequestExample(typeof(CreateSeatCommand), typeof(CreateSeatCommandExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Create(
		[FromBody] CreateSeatCommand request,
		CancellationToken cancellationToken)
	{
		var seat = await mediator.Send(request, cancellationToken);

		return Ok(seat);
	}

	[HttpPatch("/seats")]
	[SwaggerRequestExample(typeof(UpdateSeatCommand), typeof(UpdateSeatCommandExample))]
	public async Task<IActionResult> Update(
		[FromBody] UpdateSeatCommand request,
		CancellationToken cancellationToken)
	{
		var seat = await mediator.Send(request, cancellationToken);

		return Ok(seat);
	}

	[HttpGet("/seats/types")]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		var seatTypes = await mediator.Send(new GetAllSeatTypesQuery(), cancellationToken);

		return Ok(seatTypes);
	}

	[HttpGet("/seats/types/hall/{hallId:Guid}")]
	public async Task<IActionResult> GetByHall(
		[FromRoute] Guid hallId,
		CancellationToken cancellationToken)
	{
		var seatTypes = await mediator.Send(
			new GetSeatTypesByHallIdQuery(hallId),
			cancellationToken);

		return Ok(seatTypes);
	}

	[HttpPost("/seats/types")]
	[SwaggerRequestExample(typeof(CreateSeatTypeCommand), typeof(CreateSeatTypeCommandExample))]
	public async Task<IActionResult> Create(
		[FromBody] CreateSeatTypeCommand request,
		CancellationToken cancellationToken)
	{
		var type = await mediator.Send(request, cancellationToken);

		return Ok(type);
	}

	[HttpPatch("/seats/types")]
	[SwaggerRequestExample(typeof(CreateSeatCommand), typeof(CreateSeatCommandExample))]
	public async Task<IActionResult> Update(
		[FromBody] UpdateSeatTypeCommand request,
		CancellationToken cancellationToken)
	{
		var type = await mediator.Send(request, cancellationToken);

		return Ok(type);
	}

	[HttpDelete("/seats/types/{id:Guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new DeleteSeatTypeCommand(id), cancellationToken);

		return NoContent();
	}
}