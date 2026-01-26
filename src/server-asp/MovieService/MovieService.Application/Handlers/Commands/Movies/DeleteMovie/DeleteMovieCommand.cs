using MediatR;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteMovie;

public class DeleteMovieCommand(Guid id) : IRequest
{
	public Guid Id { get; private set; } = id;
}