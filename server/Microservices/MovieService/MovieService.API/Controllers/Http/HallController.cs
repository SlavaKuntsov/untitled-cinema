using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts;
using MovieService.API.Contracts.Examples.Movies;
using MovieService.API.Contracts.RequestExamples.Halls;
using MovieService.API.Contracts.Requests.Halls;
using MovieService.Application.Handlers.Commands.Halls.CreateHall;
using MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;
using MovieService.Application.Handlers.Commands.Halls.DeleteHall;
using MovieService.Application.Handlers.Commands.Halls.UpdateHall;
using MovieService.Application.Handlers.Queries.Halls.GetAllHalls;
using MovieService.Application.Handlers.Queries.Halls.GetHallById;
using MovieService.Domain.Exceptions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
//[Authorize(Policy = "AdminOnly")]
public class HallController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public HallController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	[HttpGet("/Halls")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Get()
	{
		var halls = await _mediator.Send(new GetAllHallsQuery());

		return Ok(halls);
	}

	[HttpGet("/Halls/{id:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid id)
	{
		var halls = await _mediator.Send(new GetHallByIdQuery(id))
			?? throw new NotFoundException($"Hall with id '{id.ToString()}' not found");

		return Ok(halls);
	}

	[HttpPost("/Halls/Simple")]
	[SwaggerRequestExample(typeof(CreateSimpleHallRequest), typeof(CreateSimpleHallRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateSimpleHallCommand requests)
	{
		var movie = await _mediator.Send(requests);

		return Ok(movie);
	}

	[HttpPost("/Halls/Custom")]
	[SwaggerRequestExample(typeof(CreateCustomHallRequest), typeof(CreateCustomHallRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateCustomHallCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch("/Halls")]
	[SwaggerRequestExample(typeof(UpdateHallRequest), typeof(UpdateHallRequestExample))]
	public async Task<IActionResult> Update([FromBody] UpdateHallCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete("/Halls/{id:Guid}")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteHallCommand(id));

		return NoContent();
	}
}