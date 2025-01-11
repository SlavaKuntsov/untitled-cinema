using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.Examples.Movies;
using MovieService.API.Contracts.Requests.Movies;
using MovieService.Application.Handlers.Commands.Movies.CreateMovie;
using MovieService.Application.Handlers.Commands.Movies.DeleteMovie;
using MovieService.Application.Handlers.Commands.Movies.UpdateMovie;
using MovieService.Application.Handlers.Queries.Movies.GetAllMovies;
using MovieService.Application.Handlers.Queries.Movies.GetMovieById;
using MovieService.Domain.Exceptions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public MovieController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	[HttpGet("/Movies")]
	public async Task<IActionResult> Get(
		[FromQuery] byte limit = 10,
		[FromQuery] byte offset = 1,
		[FromQuery] string? filter = null,
		[FromQuery] string? filterValue = null,
		[FromQuery] string sortBy = "title",
		[FromQuery] string sortDirection = "asc")
	{
		var movies = await _mediator.Send(new GetAllMoviesQuery(
			limit,
			offset,
			filter,
			filterValue,
			sortBy,
			sortDirection));

		return Ok(movies);
	}

	[HttpGet("/Movies/{id:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid id)
	{
		var movies = await _mediator.Send(new GetMovieByIdQuery(id))
			?? throw new NotFoundException($"Movie with id '{id.ToString()}' not found");

		return Ok(movies);
	}

	[HttpPost("/Movies")]
	[SwaggerRequestExample(typeof(CreateMovieRequest), typeof(CreateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Create([FromBody] CreateMovieCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch("/Movies")]
	[SwaggerRequestExample(typeof(UpdateMovieRequest), typeof(UpdateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateMovieCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete("/Movies/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteMovieCommand(id));

		return NoContent();
	}
}