using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetAllDays;

public partial class GetAllDaysQuery() : IRequest<IList<DayModel>>
{

}