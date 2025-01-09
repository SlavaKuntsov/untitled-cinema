using System.Diagnostics;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MovieService.API.Contracts.RequestExamples.Days;
using MovieService.API.Contracts.Requests.Days;
using MovieService.Application.Handlers.Commands.Days.CreateSession;
using MovieService.Application.Handlers.Queries.Days.GetDayByDate;
using MovieService.Domain.Exceptions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class DayController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public DayController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	[HttpGet("/Days/{date}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetDay([FromRoute] string date = "05-01-2025")
	{
		// TODO - возможно добавить range Для даты от - до
		var day = await _mediator.Send(new GetDayByDateQuery(date))
			?? throw new NotFoundException(message: $"Day '{date}' not found");

		return Ok(day);
	}

	[HttpPost("/Days")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[SwaggerRequestExample(typeof(CreateDayRequest), typeof(CreateDayRequestExample))]
	public async Task<IActionResult> Create([FromBody] CreateDayCommand request)
	{
		var movie = await _mediator.Send(request);

		return Ok(movie);
	}
}