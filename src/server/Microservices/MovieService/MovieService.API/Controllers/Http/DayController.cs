﻿using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieService.API.Contracts.RequestExamples.Days;
using MovieService.Application.Handlers.Commands.Days.CreateSession;
using MovieService.Application.Handlers.Commands.Days.DeleteDay;
using MovieService.Application.Handlers.Queries.Days.GetAllDays;
using MovieService.Application.Handlers.Queries.Days.GetDayByDate;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class DayController(IMediator mediator) : ControllerBase
{
	[HttpGet("/days")]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		var day = await mediator.Send(new GetAllDaysQuery(), cancellationToken);

		return Ok(day);
	}

	[HttpGet("/days/{date}")]
	public async Task<IActionResult> Get(
		CancellationToken cancellationToken,
		[FromRoute] string date = "05-01-2025")
	{
		var day = await mediator.Send(new GetDayByDateQuery(date), cancellationToken)
				?? throw new NotFoundException($"Day '{date}' not found.");

		return Ok(day);
	}

	[HttpPost("/days")]
	[SwaggerRequestExample(typeof(CreateDayCommand), typeof(CreateDayRequestExample))]
	public async Task<IActionResult> Create(
		[FromBody] CreateDayCommand request,
		CancellationToken cancellationToken)
	{
		var movie = await mediator.Send(request, cancellationToken);

		return Ok(movie);
	}


	[HttpDelete("/days/{id:Guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new DeleteDayCommand(id), cancellationToken);

		return NoContent();
	}
}