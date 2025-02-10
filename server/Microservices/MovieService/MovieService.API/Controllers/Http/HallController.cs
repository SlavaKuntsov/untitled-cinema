using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts;
using MovieService.API.Contracts.Examples.Movies;
using MovieService.API.Contracts.RequestExamples.Halls;
using MovieService.Application.Handlers.Commands.Halls.CreateHall;
using MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;
using MovieService.Application.Handlers.Commands.Halls.DeleteHall;
using MovieService.Application.Handlers.Commands.Halls.UpdateHall;
using MovieService.Application.Handlers.Queries.Halls.GetAllHalls;
using MovieService.Application.Handlers.Queries.Halls.GetHallById;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
//[Authorize(Policy = "AdminOnly")]
public class HallController : ControllerBase
{
	private readonly IMediator _mediator;

	public HallController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/halls")]
	public async Task<IActionResult> Get()
	{
		var halls = await _mediator.Send(new GetAllHallsQuery());

		return Ok(halls);
	}

	[HttpGet("/halls/{id:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid id)
	{
		var halls = await _mediator.Send(new GetHallByIdQuery(id));

		return Ok(halls);
	}

	[HttpPost("/halls/simple")]
	[SwaggerRequestExample(typeof(CreateSimpleHallCommand), typeof(CreateSimpleHallRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateSimpleHallCommand requests)
	{
		var movie = await _mediator.Send(requests);

		return Ok(movie);
	}

	[HttpPost("/halls/custom")]
	[SwaggerRequestExample(typeof(CreateCustomHallCommand), typeof(CreateCustomHallRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateCustomHallCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch("/halls")]
	[SwaggerRequestExample(typeof(UpdateHallCommand), typeof(UpdateHallRequestExample))]
	public async Task<IActionResult> Update([FromBody] UpdateHallCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete("/halls/{id:Guid}")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteHallCommand(id));

		return NoContent();
	}
}