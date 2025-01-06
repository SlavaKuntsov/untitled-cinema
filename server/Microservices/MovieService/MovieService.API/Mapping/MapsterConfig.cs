using System.Globalization;

using Mapster;

using MovieService.Application.Handlers.Commands.Halls.UpdateHall;
using MovieService.Application.Handlers.Commands.Movies.UpdateMovie;
using MovieService.Domain;
using MovieService.Domain.Models;

namespace MovieService.API.Mapping;

public class MapsterConfig : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.NewConfig<UpdateMovieCommand, MovieModel>()
			.Map(dest => dest.Title, src => src.Title)
			.Map(dest => dest.Genres, src => src.Genres)
			.Map(dest => dest.Description, src => src.Description)
			.Map(dest => dest.DurationMinutes, src => src.DurationMinutes)
			.Map(dest => dest.Producer, src => src.Producer)
			.Map(dest => dest.ReleaseDate, src => ParseDateOrDefault(src.ReleaseDate));

		config.NewConfig<UpdateHallCommand, HallModel>()
			.Map(dest => dest.Name, src => src.Name)
			.Map(dest => dest.TotalSeats, src => src.TotalSeats);
	}

	private static DateTime ParseDateOrDefault(string dateOfBirthString)
	{
		return DateTime.TryParseExact(
			dateOfBirthString,
			Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var parsedDateTime)
			? parsedDateTime
			: DateTime.MinValue;
	}
}