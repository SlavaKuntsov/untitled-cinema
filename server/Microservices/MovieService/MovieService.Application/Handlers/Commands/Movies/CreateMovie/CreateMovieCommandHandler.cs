using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public class CreateMovieCommandHandler(
	IMoviesRepository moviesRepository,
	IMovieGenresRepository movieGenresRepository,
	IMapper mapper) : IRequestHandler<CreateMovieCommand, Guid>
{
	private readonly IMoviesRepository _moviesRepository = moviesRepository;
	private readonly IMovieGenresRepository _movieGenresRepository = movieGenresRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
	{
		if (!request.ReleaseDate.DateTimeFormatTryParse(out DateTime parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		parsedDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Local).ToUniversalTime();

		DateTime dateNow = DateTime.UtcNow;

		var movie = new MovieModel(
			Guid.NewGuid(),
			request.Title,
			request.Description,
			request.DurationMinutes,
			request.Producer,
			parsedDateTime,
			dateNow,
			dateNow);

		var genreEntities = new List<GenreEntity>();

		foreach (var genreName in request.Genres)
		{
			var existingGenre = await _movieGenresRepository.GetByNameAsync(genreName, cancellationToken);

			if (existingGenre == null)
			{
				var genre = new GenreModel(Guid.NewGuid(), genreName);

				var genreEntity = _mapper.Map<GenreEntity>(genre);

				await _movieGenresRepository.AddAsync(genreEntity, cancellationToken);
				genreEntities.Add(genreEntity);
			}
			else
			{
				genreEntities.Add(existingGenre);
			}
		}

		var movieEntity = _mapper.Map<MovieEntity>(movie);

		movieEntity.MovieGenres = genreEntities.Select(genre => new MovieGenreEntity
		{
			MovieId = movieEntity.Id,
			GenreId = genre.Id,
			Genre = genre
		}).ToList();

		await _moviesRepository.CreateAsync(movieEntity, cancellationToken);

		return movie.Id;
	}
}