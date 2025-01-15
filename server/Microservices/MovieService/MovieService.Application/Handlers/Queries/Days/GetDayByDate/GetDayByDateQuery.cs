using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetDayByDate;

public class GetDayByDateQuery(string date) : IRequest<DayModel>
{
	public string Date { get; private set; } = date;
}