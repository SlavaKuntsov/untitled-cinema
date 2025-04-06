using MediatR;
using Microsoft.AspNetCore.Mvc;
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
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class MovieController(
	IMediator mediator,
	ILogger<MovieController> logger) : ControllerBase
{
	[HttpGet("/movies")]
	public async Task<IActionResult> Get(
		[FromQuery] GetMovieRequest request,
		CancellationToken cancellationToken)
	{
		logger.LogInformation("Fetch all movies.");

		var paginatedMovies = await mediator.Send(
			new GetAllMoviesQuery(
				request.Limit,
				request.Offset,
				request.Filters,
				request.FilterValues,
				request.SortBy,
				request.SortDirection,
				request.Date),
			cancellationToken);

		var (nextRef, prevRef) = GeneratePaginationLinks(request, paginatedMovies.Total);

		var newPaginatedMovies = paginatedMovies with { NextRef = nextRef, PrevRef = prevRef };

		logger.LogInformation("Successfully fetched {Count} movies.", request.Limit);

		return Ok(newPaginatedMovies);
	}

	[HttpGet("/movies/{id:Guid}")]
	public async Task<IActionResult> Get(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		var movies = await mediator.Send(new GetMovieByIdQuery(id), cancellationToken);

		return Ok(movies);
	}

	[HttpPost("/movies")]
	[SwaggerRequestExample(typeof(CreateMovieCommand), typeof(CreateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Create(
		[FromBody] CreateMovieCommand request,
		CancellationToken cancellationToken)
	{
		var movie = await mediator.Send(request, cancellationToken);

		return Ok(movie);
	}

	[HttpPatch("/movies")]
	[SwaggerRequestExample(typeof(UpdateMovieCommand), typeof(UpdateMovieRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update(
		[FromBody] UpdateMovieCommand request,
		CancellationToken cancellationToken)
	{
		var movie = await mediator.Send(request, cancellationToken);

		return Ok(movie);
	}

	[HttpDelete("/movies/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new DeleteMovieCommand(id), cancellationToken);

		return NoContent();
	}

	[HttpGet("/movies/genres")]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		var genres = await mediator.Send(new GetAllGenresQuery(), cancellationToken);

		return Ok(genres);
	}

	[HttpPatch("/movies/genres")]
	[SwaggerRequestExample(typeof(UpdateGenreCommand), typeof(UpdateGenreCommandExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Update(
		[FromBody] UpdateGenreCommand request,
		CancellationToken cancellationToken)
	{
		var genre = await mediator.Send(request, cancellationToken);

		return Ok(genre);
	}

	[HttpDelete("/movies/genres/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> DeleteGenre(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new DeleteGenreCommand(id), cancellationToken);

		return NoContent();
	}

	private (string NextRef, string PrevRef) GeneratePaginationLinks(GetMovieRequest request, int totalItems)
	{
		var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

		var nextOffset = request.Offset + 1;

		var nextRef = request.Offset * request.Limit < totalItems
			? $"{baseUrl}?Limit={request.Limit}&Offset={nextOffset}&SortBy={request.SortBy}&SortDirection={request.SortDirection}"
			: string.Empty;

		var prevRef = string.Empty;

		if (request.Offset > 1)
		{
			var prevOffset = request.Offset - 1;

			prevRef =
				$"{baseUrl}?Limit={request.Limit}&Offset={prevOffset}&SortBy={request.SortBy}&SortDirection={request.SortDirection}";
		}

		return (nextRef, prevRef);
	}
}