using System.Globalization;
using System.Text.Json;

using Mapster;

using MovieService.Application.Handlers.Commands.Halls.UpdateHall;
using MovieService.Application.Handlers.Commands.Movies.UpdateMovie;
using MovieService.Domain;
using MovieService.Domain.Entities;
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

		config.NewConfig<UpdateHallCommand, HallEntity>()
			 .Map(dest => dest.Id, src => src.Id)
			 .Map(dest => dest.Name, src => src.Name)
			 .Map(dest => dest.TotalSeats, src => src.TotalSeats)
			.Map(dest => dest.SeatsArrayJson, src => SerializeSeatsArray(src.Seats));

		config.NewConfig<MovieModel, MovieEntity>()
			.Map(dest => dest.MovieGenres, src => new List<MovieGenreEntity>());

		config.NewConfig<MovieEntity, MovieModel>()
			.Map(dest => dest.Genres,
				 src => src.MovieGenres.Select(mg => mg.Genre.Name).ToList());

		config.NewConfig<HallEntity, HallModel>()
			.Map(dest => dest.SeatsArray, src => DeserializeSeatsArray(src.SeatsArrayJson));

		config.NewConfig<HallModel, HallEntity>()
			.Map(dest => dest.SeatsArrayJson, src => SerializeSeatsArray(src.SeatsArray));
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

	private static int[][] DeserializeSeatsArray(string json)
	{
		return string.IsNullOrWhiteSpace(json)
			? []
			: JsonSerializer.Deserialize<int[][]>(json) ?? [];
	}

	private static string SerializeSeatsArray(int[][] seats)
	{
		return JsonSerializer.Serialize(seats);
	}
}