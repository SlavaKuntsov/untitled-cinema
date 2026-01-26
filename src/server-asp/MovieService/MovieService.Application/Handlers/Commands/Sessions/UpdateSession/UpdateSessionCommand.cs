using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Sessions.UpdateSession;

public class UpdateSessionCommand(
	Guid id,
	Guid movieId,
	Guid hallId,
	string startTime) : IRequest<SessionModel>
{
	public Guid Id { get; private set; } = id;
	public Guid MovieId { get; private set; } = movieId;
	public Guid HallId { get; private set; } = hallId;
	public string StartTime { get; private set; } = startTime;
}