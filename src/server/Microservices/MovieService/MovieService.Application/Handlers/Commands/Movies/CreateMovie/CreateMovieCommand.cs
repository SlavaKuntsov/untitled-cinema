using MediatR;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public record CreateMovieCommand(
	string Title,
	IList<string> Genres,
	string Description,
	byte[] Poster,
	decimal Price,
	short DurationMinutes,
	string Producer,
	string InRoles,
	byte AgeLimit,
	string ReleaseDate) : IRequest<Guid>;