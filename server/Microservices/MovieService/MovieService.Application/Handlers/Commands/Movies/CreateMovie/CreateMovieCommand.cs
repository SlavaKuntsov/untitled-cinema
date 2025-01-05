using MediatR;

namespace MovieService.Application.Handlers.Commands.Movies.Create;

public class CreateMovieCommand(
	string title,
	IList<string> genres,
	string description,
	short durationMinutes,
	string producer,
	string releaseDate) : IRequest<Guid>
{
	public string Title { get; private set; } = title;
	public IList<string> Genres { get; private set; } = genres;
	public string Description { get; private set; } = description;
	public short DurationMinutes { get; private set; } = durationMinutes;
	public string Producer { get; private set; } = producer;
	public string ReleaseDate { get; private set; } = releaseDate;
}