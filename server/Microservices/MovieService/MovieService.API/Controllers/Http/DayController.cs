using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Days;
using MovieService.Application.Handlers.Commands.Days.CreateSession;
using MovieService.Application.Handlers.Commands.Days.DeleteDay;
using MovieService.Application.Handlers.Queries.Days.GetAllDays;
using MovieService.Application.Handlers.Queries.Days.GetDayByDate;
using MovieService.Domain.Exceptions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class DayController : ControllerBase
{
	private readonly IMediator _mediator;

	public DayController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/Days")]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		var day = await _mediator.Send(new GetAllDaysQuery(), cancellationToken);

		return Ok(day);
	}

	[HttpGet("/Days/{date}")]
	public async Task<IActionResult> Get(
		CancellationToken cancellationToken,
		[FromRoute] string date = "05-01-2025")
	{
		var day = await _mediator.Send(new GetDayByDateQuery(date), cancellationToken)
			?? throw new NotFoundException(message: $"Day '{date}' not found.");

		return Ok(day);
	}

	[HttpPost("/Days")]
	[SwaggerRequestExample(typeof(CreateDayCommand), typeof(CreateDayRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateDayCommand request, CancellationToken cancellationToken)
	{
		var movie = await _mediator.Send(request, cancellationToken);

		return Ok(movie);
	}


	[HttpDelete("/Days/{id:Guid}")]
	public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		await _mediator.Send(new DeleteDayCommand(id), cancellationToken);

		return NoContent();
	}
}