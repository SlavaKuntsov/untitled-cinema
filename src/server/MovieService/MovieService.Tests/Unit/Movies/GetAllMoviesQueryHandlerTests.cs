using System.Collections.ObjectModel;
using Bogus;
using Domain.DTOs;
using Domain.Exceptions;
using FluentAssertions;
using MapsterMapper;
using Moq;
using MovieService.Application.Handlers.Queries.Movies.GetAllMovies;
using MovieService.Domain.Entities;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;
using Redis.Services;
using Xunit;

namespace MovieService.Tests.Unit.Movies;

public class GetAllMoviesQueryHandlerTests
{
	private readonly Faker _faker = new();

	private readonly GetAllMoviesQueryHandler _handler;

	private readonly Mock<IMapper> _mapperMock = new();

	private readonly Mock<IRedisCacheService> _redisCacheServiceMock = new();

	private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

	public GetAllMoviesQueryHandlerTests()
	{
		_handler = new GetAllMoviesQueryHandler(
			_unitOfWorkMock.Object,
			_redisCacheServiceMock.Object,
			_mapperMock.Object
		);
	}

	[Fact]
	public async Task Handle_ValidRequest_ReturnsPaginatedMovies()
	{
		// Arrange
		var request = new GetAllMoviesQuery(
			10,
			1,
			new[] { "genre" },
			new[] { "action" },
			"title",
			"asc",
			null);
		var movieEntities = GenerateMovieEntities(10);
		var movieModels = GenerateMovieModels(10);
		var totalMovies = movieEntities.Count;

		_unitOfWorkMock
			.Setup(x => x.MoviesRepository.Get())
			.Returns(movieEntities.AsQueryable());

		_unitOfWorkMock
			.Setup(x => x.MoviesRepository.GetCount(It.IsAny<IQueryable<MovieEntity>>()))
			.ReturnsAsync(totalMovies);

		_unitOfWorkMock
			.Setup(
				x => x.MoviesRepository.ToListAsync(
					It.IsAny<IQueryable<MovieEntity>>(),
					It.IsAny<CancellationToken>()))
			.ReturnsAsync(movieEntities);

		_mapperMock
			.Setup(x => x.Map<IList<MovieModel>>(It.IsAny<IList<MovieEntity>>()))
			.Returns(movieModels);

		// Act
		var result = await _handler.Handle(request, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Total.Should().Be(totalMovies);
	}

	[Fact]
	public async Task Handle_InvalidDateFormat_ThrowsBadRequestException()
	{
		// Arrange
		var request = new GetAllMoviesQuery(
			10,
			1,
			new[] { "genre" },
			new[] { "action" },
			"title",
			"asc",
			"10-10-2000");

		// Act & Assert
		await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(request, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_FiltersAndFilterValuesMismatch_ThrowsInvalidOperationException()
	{
		// Arrange
		var request = new GetAllMoviesQuery(
			10,
			1,
			new[] { "genre" },
			new[] { "action", "comedy" },
			"title",
			"asc",
			null);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(
			() => _handler.Handle(request, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_CachedMovies_ReturnsCachedMovies()
	{
		// Arrange
		var request = new GetAllMoviesQuery(
			10,
			1,
			new[] { "genre" },
			new[] { "action" },
			"title",
			"asc",
			null);
		var movieModels = GenerateMovieModels(10);
		var totalMovies = movieModels.Count;
		var cacheKey = GenerateCacheKey(request);

		_redisCacheServiceMock
			.Setup(x => x.GetValueAsync<IList<MovieModel>?>(cacheKey))
			.ReturnsAsync(movieModels);

		// Act
		var result = await _handler.Handle(request, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Total.Should().Be(totalMovies);
	}

	private List<MovieEntity> GenerateMovieEntities(int count)
	{
		var movieEntities = new List<MovieEntity>();

		for (var i = 0; i < count; i++)
			movieEntities.Add(
				new MovieEntity
				{
					Id = Guid.NewGuid(),
					Title = _faker.Lorem.Sentence(),
					Producer = _faker.Name.FullName(),
					DurationMinutes = (short)_faker.Random.Int(60, 180),
					AgeLimit = (byte)_faker.Random.Int(0, 18),
					ReleaseDate = _faker.Date.Past(),
					MovieGenres = new Collection<MovieGenreEntity>
						{ new() { Genre = new GenreEntity { Name = "Action" } } }
				});

		return movieEntities;
	}

	private List<MovieModel> GenerateMovieModels(int count)
	{
		var movieModels = new List<MovieModel>();

		for (var i = 0; i < count; i++)
			movieModels.Add(
				new MovieModel(
					Guid.NewGuid(),
					_faker.Lorem.Sentence(),
					_faker.Lorem.Sentence(),
					0.00m,
					(short)_faker.Random.Int(60, 180),
					_faker.Name.FullName(),
					_faker.Name.FullName(),
					(byte)_faker.Random.Int(0, 18),
					_faker.Date.Past(),
					DateTime.UtcNow,
					DateTime.UtcNow
				));

		return movieModels;
	}

	private string GenerateCacheKey(GetAllMoviesQuery request)
	{
		var filters = request.Filters
			.Zip(request.FilterValues, (field, value) => new FilterDto(field, value))
			.ToList();

		return
			$"movies_{string.Join("_", filters.GroupBy(f => f.Field).Select(g => $"{g.Key}_{string.Join("-", g.Select(f => f.Value))}"))}_{request.SortBy}_{request.SortDirection}_{request.Offset}_{request.Limit}_{request.Date}"
				.Replace("\r", "")
				.Replace("\n", "")
				.Replace(" ", "");
	}
}