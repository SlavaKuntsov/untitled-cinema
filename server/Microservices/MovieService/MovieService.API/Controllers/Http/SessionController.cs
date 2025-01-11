using System.Globalization;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Sessions;
using MovieService.API.Contracts.Requests.Sessions;
using MovieService.Application.Handlers.Commands.Movies.DeleteMovie;
using MovieService.Application.Handlers.Commands.Sessoins.FillSession;
using MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;
using MovieService.Domain.Exceptions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public SessionController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	[HttpGet("/Sessions")]
	public async Task<IActionResult> Get(
		[FromQuery] byte limit = 10,
		[FromQuery] byte offset = 1,
		[FromQuery] string? date = null,
		[FromQuery] string? hall = null)
	{
		var movies = await _mediator.Send(new GetAllSessionsQuery(
			limit,
			offset,
			date,
			hall));

		return Ok(movies);
	}

	[HttpPost("/Sessions")]
	[SwaggerRequestExample(typeof(FillSessionRequest), typeof(FillSessionRequestExample))]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Fill([FromBody] FillSessionCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}

	//[HttpPatch(nameof(Update))]
	//[ProducesResponseType(StatusCodes.Status200OK)]
	//[ProducesResponseType(StatusCodes.Status400BadRequest)]
	//[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[SwaggerRequestExample(typeof(UpdateMovieRequest), typeof(UpdateMovieRequestExample))]
	////[Authorize(Policy = "AdminOnly")]
	//public async Task<IActionResult> Update([FromBody] UpdateMovieCommand request)
	//{
	//	var movie = await _mediator.Send(request);

	//	return Ok(movie);
	//}

	[HttpDelete("/Sessions/{id:Guid}")]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteMovieCommand(id));

		return NoContent();
	}
}