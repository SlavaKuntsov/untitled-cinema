using MediatR;

namespace MovieService.Application.Handlers.Commands.Sessions.DeleteSession;

public class DeleteSessionCommand(Guid id) : IRequest
{
	public Guid Id { get; private set; } = id;
}