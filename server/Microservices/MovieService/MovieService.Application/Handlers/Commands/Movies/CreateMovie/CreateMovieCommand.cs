using MediatR;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public class CreateMovieCommand(
    string title,
    IList<string> genres,
    string description,
    decimal price,
    short durationMinutes,
    string producer,
    byte ageLimit,
    string releaseDate) : IRequest<Guid>
{
    public string Title { get; private set; } = title;
    public IList<string> Genres { get; private set; } = genres;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public short DurationMinutes { get; private set; } = durationMinutes;
    public string Producer { get; private set; } = producer;
	public byte AgeLimit { get; private set; } = ageLimit;
	public string ReleaseDate { get; private set; } = releaseDate;
}