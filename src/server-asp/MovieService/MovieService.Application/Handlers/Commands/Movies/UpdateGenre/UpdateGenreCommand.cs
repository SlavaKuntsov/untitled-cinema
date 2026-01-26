using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateGenre;

public class UpdateGenreCommand(
	Guid id,
	string name) : IRequest<GenreModel>
{
	public Guid Id { get; private set; } = id;
	public string Name { get; private set; } = name;
}