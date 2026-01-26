using MediatR;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteGenre;

public class DeleteGenreCommand(Guid id) : IRequest
{
	public Guid Id { get; private set; } = id;
}