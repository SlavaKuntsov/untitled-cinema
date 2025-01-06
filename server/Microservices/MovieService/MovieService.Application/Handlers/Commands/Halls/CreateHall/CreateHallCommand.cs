using MediatR;

namespace MovieService.Application.Handlers.Commands.Halls.CreateHall;

public class CreateHallCommand(
	string name,
	short totalSeats) : IRequest<Guid>
{
	public string Name { get; private set; } = name;
	public short TotalSeats { get; private set; } = totalSeats;
}