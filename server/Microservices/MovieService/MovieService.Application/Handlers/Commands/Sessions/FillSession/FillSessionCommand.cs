using MediatR;

namespace MovieService.Application.Handlers.Commands.Sessions.FillSession;

public class FillSessionCommand(
	Guid movieId,
	Guid hallId,
	decimal priceModifier,
	string startTime) : IRequest<Guid>
{
	public Guid MovieId { get; private set; } = movieId;
	public Guid HallId { get; private set; } = hallId;
	public decimal PriceModifier { get; private set; } = priceModifier;
	public string StartTime { get; private set; } = startTime;
}