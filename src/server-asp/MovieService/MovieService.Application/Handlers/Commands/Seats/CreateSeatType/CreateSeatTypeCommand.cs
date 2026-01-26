using MediatR;

namespace MovieService.Application.Handlers.Commands.Seats.CreateSeatType;

public class CreateSeatTypeCommand(
	string name,
	decimal priceModifier) : IRequest<Guid>
{
	public string Name { get; private set; } = name;
	public decimal PriceModifier { get; private set; } = priceModifier;
}