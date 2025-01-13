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
	public async Task<IActionResult> Get()
	{
		var day = await _mediator.Send(new GetAllDaysQuery());

		return Ok(day);
	}

	[HttpGet("/Days/{date}")]
	public async Task<IActionResult> Get([FromRoute] string date = "05-01-2025")
	{
		var day = await _mediator.Send(new GetDayByDateQuery(date))
			?? throw new NotFoundException(message: $"Day '{date}' not found");

		return Ok(day);
	}

	[HttpPost("/Days")]
	[SwaggerRequestExample(typeof(CreateDayCommand), typeof(CreateDayRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateDayCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}


	[HttpDelete("/Days/{id:Guid}")]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		await _mediator.Send(new DeleteDayCommand(id));

		return NoContent();
	}
}