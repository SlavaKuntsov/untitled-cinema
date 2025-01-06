using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetAllHalls;

public partial class GetAllHallsQuery() : IRequest<IList<HallModel>>
{

}