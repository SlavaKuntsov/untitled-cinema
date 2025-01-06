using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Sessions;
using MovieService.API.Contracts.Requests.Sessions;
using MovieService.Application.Handlers.Commands.Movies.DeleteMovie;
using MovieService.Application.Handlers.Commands.Sessoins.FillSession;
using MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;
using MovieService.Application.Handlers.Queries.Sessoins.GetSessionByDate;
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
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetSessions()
	{
		var movies = await _mediator.Send(new GetAllSessionsQuery());

		return Ok(movies);
	}

	[HttpGet("/Sessions/{date}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetSession([FromRoute] string date)
	{
		var session = await _mediator.Send(new GetSessionByDateQuery(date))
			?? throw new NotFoundException($"Session '{date}' not found");

		return Ok(session);
	}

	[HttpPost("/Sessions")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteMovieCommand(id));

		return Ok();
	}
}