using MediatR;

namespace MovieService.Application.Handlers.Commands.Sessoins.FillSession;

public class FillSessionCommand(
	Guid movieId,
	Guid hallId,
	string startTime) : IRequest<Guid>
{
	public Guid MovieId { get; private set; } = movieId;
	public Guid HallId { get; private set; } = hallId;
	public string StartTime { get; private set; } = startTime;
}