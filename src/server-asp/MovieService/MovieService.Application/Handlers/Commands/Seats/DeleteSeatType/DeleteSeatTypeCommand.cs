using MediatR;

namespace MovieService.Application.Handlers.Commands.Seats.DeleteSeatType;

public class DeleteSeatTypeCommand(Guid id) : IRequest
{
	public Guid Id { get; private set; } = id;
}