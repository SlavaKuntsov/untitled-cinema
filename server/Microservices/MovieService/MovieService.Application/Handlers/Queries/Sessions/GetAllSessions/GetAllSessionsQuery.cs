using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;

public partial class GetAllSessionsQuery() : IRequest<IList<SessionModel>>
{

}