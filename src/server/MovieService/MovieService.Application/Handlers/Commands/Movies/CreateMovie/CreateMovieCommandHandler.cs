using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Minios.Services;
using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;
using Redis.Services;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public record CreateMovieCommand(
	string Title,
	IList<string> Genres,
	string Description,
	decimal Price,
	short DurationMinutes,
	string Producer,
	string InRoles,
	byte AgeLimit,
	string ReleaseDate) : IRequest<Guid>;

public class CreateMovieCommandHandler(
	IMinioService minioService,
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<CreateMovieCommand, Guid>
{
	public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
	{
		if (!request.ReleaseDate.DateTimeFormatTryParse(out var parsedDateTime))
			throw new BadRequestException("Invalid date format.");
		
		var dateNow = DateTime.UtcNow;

		var movie = new MovieModel(
			Guid.NewGuid(),
			request.Title,
			request.Description,
			request.Price,
			request.DurationMinutes,
			request.Producer,
			request.InRoles,
			request.AgeLimit,
			parsedDateTime,
			dateNow,
			dateNow);

		var genreEntities = new List<GenreEntity>();

		foreach (var genreName in request.Genres)
		{
			var existingGenre =
				await unitOfWork.MoviesRepository.GetGenreByNameAsync(genreName, cancellationToken);

			if (existingGenre == null)
			{
				var genre = new GenreModel(Guid.NewGuid(), genreName);

				var genreEntity = mapper.Map<GenreEntity>(genre);

				await unitOfWork.Repository<GenreEntity>().CreateAsync(genreEntity, cancellationToken);
				genreEntities.Add(genreEntity);
			}
			else
			{
				genreEntities.Add(existingGenre);
			}
		}

		var movieEntity = mapper.Map<MovieEntity>(movie);

		movieEntity.MovieGenres = genreEntities.Select(
				genre => new MovieGenreEntity
				{
					MovieId = movieEntity.Id,
					GenreId = genre.Id
				})
			.ToList();

		await unitOfWork.Repository<MovieEntity>().CreateAsync(movieEntity, cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		await redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return movie.Id;
	}
}