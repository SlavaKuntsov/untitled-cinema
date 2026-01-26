using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetAllDays;

public class GetAllDaysQuery() : IRequest<IList<DayModel>>
{
}