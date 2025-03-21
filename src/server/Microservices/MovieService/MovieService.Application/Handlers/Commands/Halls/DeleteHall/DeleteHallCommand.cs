using MediatR;

namespace MovieService.Application.Handlers.Commands.Halls.DeleteHall;

public class DeleteHallCommand(Guid id) : IRequest
{
	public Guid Id { get; private set; } = id;
}