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
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetMovies()
	{
		var movies = await _mediator.Send(new GetAllMoviesQuery());

		return Ok(movies);
	}

	[HttpGet("/Movies/{id:Guid}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetMovie([FromRoute] Guid id)
	{
		var movies = await _mediator.Send(new GetMovieByIdQuery(id))
			?? throw new NotFoundException($"Movie with id '{id.ToString()}' not found");

		return Ok(movies);
	}

	[HttpPost("/Movies")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[SwaggerRequestExample(typeof(CreateMovieRequest), typeof(CreateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Create([FromBody] CreateMovieCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch("/Movies")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[SwaggerRequestExample(typeof(UpdateMovieRequest), typeof(UpdateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateMovieCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete("/Movies/{id:Guid}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteMovieCommand(id));

		return Ok();
	}
}