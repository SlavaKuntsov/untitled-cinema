using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.UpdateSeatType;

public class UpdateSeatTypeCommand(
	Guid id,
	string name,
	decimal priceModifier) : IRequest<SeatTypeModel>
{
	public Guid Id { get; set; } = id;
	public string Name { get; set; } = name;
	public decimal PriceModifier { get; set; } = priceModifier;
}