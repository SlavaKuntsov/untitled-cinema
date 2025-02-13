using System.Globalization;
using System.Text.Json;

using Mapster;

using MovieService.Application.Handlers.Commands.Halls.UpdateHall;
using MovieService.Application.Handlers.Commands.Movies.UpdateMovie;
using MovieService.Application.Handlers.Commands.Seats.UpdateSeat;
using MovieService.Application.Handlers.Commands.Seats.UpdateSeatType;
using MovieService.Application.Handlers.Commands.Sessions.UpdateSession;
using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;
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
			.Map(dest => dest.AgeLimit, src => src.AgeLimit)
			.Map(dest => dest.ReleaseDate, src => ParseDateTimeOrDefault(src.ReleaseDate));

		config.NewConfig<UpdateHallCommand, HallEntity>()
			 .Map(dest => dest.Id, src => src.Id)
			 .Map(dest => dest.Name, src => src.Name)
			 .Map(dest => dest.TotalSeats, src => src.TotalSeats)
			 .Map(dest => dest.SeatsArrayJson, src => SerializeSeatsArray(src.Seats));

		config.NewConfig<UpdateSessionCommand, SessionEntity>()
			 .Map(dest => dest.Id, src => src.Id)
			 .Map(dest => dest.MovieId, src => src.MovieId)
			 .Map(dest => dest.HallId, src => src.HallId)
			 .Map(dest => dest.StartTime, src => ParseDateTimeOrDefault(src.StartTime));

		config.NewConfig<UpdateSeatTypeCommand, SeatTypeEntity>()
			 .Map(dest => dest.Id, src => src.Id)
			 .Map(dest => dest.Name, src => src.Name)
			 .Map(dest => dest.PriceModifier, src => src.PriceModifier);

		config.NewConfig<UpdateSeatCommand, SeatEntity>()
			 .Map(dest => dest.Id, src => src.Id)
			 .Map(dest => dest.HallId, src => src.HallId)
			 .Map(dest => dest.SeatTypeId, src => src.SeatTypeId)
			 .Map(dest => dest.Row, src => src.Row)
			 .Map(dest => dest.Column, src => src.Column);

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

	private static DateTime ParseDateTimeOrDefault(string date)
	{
		return DateTime.TryParseExact(
			date,
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