using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetAllHalls;

public class GetAllHallsQuery() : IRequest<IList<HallModel>>
{
}