using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Application.Interfaces.Caching;
using MovieService.Domain;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public class CreateMovieCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<CreateMovieCommand, Guid>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
	{
		if (!request.ReleaseDate.DateTimeFormatTryParse(out DateTime parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		DateTime dateNow = DateTime.UtcNow;

		var movie = new MovieModel(
			Guid.NewGuid(),
			request.Title,
			request.Description,
			request.Price,
			request.DurationMinutes,
			request.Producer,
			parsedDateTime,
			dateNow,
			dateNow);

		var genreEntities = new List<GenreEntity>();

		foreach (var genreName in request.Genres)
		{
			var existingGenre = await _unitOfWork.MoviesRepository.GetGenreByNameAsync(genreName, cancellationToken);

			if (existingGenre == null)
			{
				var genre = new GenreModel(Guid.NewGuid(), genreName);

				var genreEntity = _mapper.Map<GenreEntity>(genre);

				await _unitOfWork.Repository<GenreEntity>().CreateAsync(genreEntity, cancellationToken);
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
			GenreId = genre.Id
		}).ToList();

		await _unitOfWork.Repository<MovieEntity>().CreateAsync(movieEntity, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		await _redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return movie.Id;
	}
}