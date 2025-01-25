using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessions.GetSessionById;

public class GetSessionByIdQuery(Guid id) : IRequest<SessionModel>
{
	public Guid Id { get; private set; } = id;
}