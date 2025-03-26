using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateMovie;

public class UpdateMovieCommand(
	Guid id,
	string title,
	IList<string> genres,
	string description,
	short durationMinutes,
	string producer,
	byte ageLimit,
	string releaseDate) : IRequest<MovieModel>
{
	public Guid Id { get; private set; } = id;
	public string Title { get; private set; } = title;
	public IList<string> Genres { get; private set; } = genres;
	public string Description { get; private set; } = description;
	public short DurationMinutes { get; private set; } = durationMinutes;
	public string Producer { get; private set; } = producer;
	public byte AgeLimit { get; private set; } = ageLimit;
	public string ReleaseDate { get; private set; } = releaseDate;
}