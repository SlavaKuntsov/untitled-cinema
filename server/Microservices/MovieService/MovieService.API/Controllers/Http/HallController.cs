using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts;
using MovieService.API.Contracts.Examples.Movies;
using MovieService.API.Contracts.Requests.Halls;
using MovieService.Application.Handlers.Commands.Halls.CreateHall;
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
	public async Task<IActionResult> GetHalls()
	{
		var halls = await _mediator.Send(new GetAllHallsQuery());

		return Ok(halls);
	}

	[HttpGet("/Hall/{id:Guid}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetMovie([FromRoute] Guid id)
	{
		var halls = await _mediator.Send(new GetHallByIdQuery(id))
			?? throw new NotFoundException($"Hall with id '{id.ToString()}' not found");

		return Ok(halls);
	}

	[HttpPost(nameof(Create))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[SwaggerRequestExample(typeof(CreateHallRequest), typeof(CreateHallRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateHallCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch(nameof(Update))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[SwaggerRequestExample(typeof(UpdateHallRequest), typeof(UpdateHallRequestExample))]
	public async Task<IActionResult> Update([FromBody] UpdateHallCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete(nameof(Delete) + "/{id:Guid}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteHallCommand(id));

		return Ok();
	}
}