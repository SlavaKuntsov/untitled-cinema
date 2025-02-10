using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.Examples.Movies;
using MovieService.API.Contracts.RequestExamples.Movies;
using MovieService.API.Contracts.Requests;
using MovieService.Application.Handlers.Commands.Movies.CreateMovie;
using MovieService.Application.Handlers.Commands.Movies.DeleteGenre;
using MovieService.Application.Handlers.Commands.Movies.DeleteMovie;
using MovieService.Application.Handlers.Commands.Movies.UpdateGenre;
using MovieService.Application.Handlers.Commands.Movies.UpdateMovie;
using MovieService.Application.Handlers.Queries.Movies.GetAllGenres;
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
	private readonly ILogger<MovieController> _logger;

	public MovieController(IMediator mediator,
		ILogger<MovieController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	[HttpGet("/movies")]
	public async Task<IActionResult> Get([FromQuery] GetMovieRequest request)
	{
		_logger.LogInformation("Fetch all movies.");

		var movies = await _mediator.Send(new GetAllMoviesQuery(
			request.Limit,
			request.Offset,
			request.Filter,
			request.FilterValue,
			request.SortBy,
			request.SortDirection));

		_logger.LogInformation("Successfully fetched {Count} movies.", movies.Count);

		return Ok(movies);
	}

	[HttpGet("/movies/{id:Guid}")]
	public async Task<IActionResult> Get([FromRoute] Guid id)
	{
		var movies = await _mediator.Send(new GetMovieByIdQuery(id))
			?? throw new NotFoundException($"Movie with id '{id.ToString()}' not found.");

		return Ok(movies);
	}

	[HttpPost("/movies")]
	[SwaggerRequestExample(typeof(CreateMovieCommand), typeof(CreateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Create([FromBody] CreateMovieCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpPatch("/movies")]
	[SwaggerRequestExample(typeof(UpdateMovieCommand), typeof(UpdateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateMovieCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	[HttpDelete("/movies/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteMovieCommand(id));

		return NoContent();
	}

	[HttpGet("/movies/genres")]
	public async Task<IActionResult> Get()
	{
		var genres = await _mediator.Send(new GetAllGenresQuery());

		return Ok(genres);
	}

	[HttpPatch("/movies/genres")]
	[SwaggerRequestExample(typeof(UpdateGenreCommand), typeof(UpdateGenreCommandExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update([FromBody] UpdateGenreCommand request)
	{
		var genre = await _mediator.Send(request);

		return Ok(genre);
	}

	[HttpDelete("/movies/genres/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> DeleteGenre([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteGenreCommand(id));

		return NoContent();
	}
}