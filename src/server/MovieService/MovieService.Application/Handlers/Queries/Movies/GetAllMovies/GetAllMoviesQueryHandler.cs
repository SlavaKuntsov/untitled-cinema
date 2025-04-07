using Brokers.Models.DTOs;
using Domain.DTOs;
using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using MovieService.Application.DTOs;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;
using Redis.Service;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public class GetAllMoviesQueryHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<GetAllMoviesQuery, PaginationWrapperDto<MovieModel>>
{
	public async Task<PaginationWrapperDto<MovieModel>> Handle(
		GetAllMoviesQuery request,
		CancellationToken cancellationToken)
	{
		if (request.Filters.Length != request.FilterValues.Length)
			throw new InvalidOperationException("The number of Filters and FilterValues must be the same.");

		var filters = request.Filters
			.Zip(request.FilterValues, (field, value) => new FilterDto(field, value))
			.ToList();

		var cacheKey = @$"movies_{
			string.Join("_", filters
				.GroupBy(f => f.Field)
				.Select(g => $"{g.Key}_{string.Join("-", g.Select(f => f.Value))}"))}_{
				request.SortBy}_{
					request.SortDirection}_{
						request.Offset}_{
							request.Limit}_{
								request.Date}"
			.Replace("\r", "")
			.Replace("\n", "")
			.Replace(" ", "");

		var totalMovies = 0;

		var cachedMovies = await redisCacheService.GetValueAsync<IList<MovieModel>?>(cacheKey);

		if (cachedMovies != null)
		{
			totalMovies = cachedMovies.Count;

			return new PaginationWrapperDto<MovieModel>(
				cachedMovies,
				request.Limit,
				request.Offset,
				totalMovies);
		}

		IQueryable<MovieEntity>? query;

		if (!string.IsNullOrWhiteSpace(request.Date))
		{
			if (!request.Date.DateFormatTryParse(out var parsedDate))
				throw new BadRequestException("Invalid date format.");

			query = unitOfWork.MoviesRepository.Get(parsedDate);
		}
		else
		{
			query = unitOfWork.MoviesRepository.Get();
		}

		foreach (var filter in filters)
			if (!string.IsNullOrWhiteSpace(filter.Field) && !string.IsNullOrWhiteSpace(filter.Value))
			{
				if (filter.Field.ToLower() == "genre")
				{
					var genreFilters = filters
						.Where(f => f.Field.ToLower() == "genre")
						.Select(f => f.Value.ToLower())
						.ToList();

					query = unitOfWork.MoviesRepository.FilterByGenre(query, genreFilters);

					break;
				}

				query = filter.Field.ToLower() switch
				{
					"producer" => query.Where(
						m =>
							m.Producer.ToLower().Contains(filter.Value.ToLower())),
					"title" => query.Where(
						m =>
							m.Title.ToLower().Contains(filter.Value.ToLower())),
					_ => throw new InvalidOperationException($"Invalid filter field '{filter.Field}'.")
				};
			}

		totalMovies = await unitOfWork.MoviesRepository.GetCount(query);

		if (request.SortDirection.ToLower() == "asc")
			query = request.SortBy.ToLower() switch
			{
				"title" => query.OrderBy(m => m.Title),
				"durationMinutes" => query.OrderBy(m => m.DurationMinutes),
				"producer" => query.OrderBy(m => m.Producer),
				"age" => query.OrderBy(m => m.AgeLimit),
				"release" => query.OrderBy(m => m.ReleaseDate),
				_ => throw new InvalidOperationException($"Invalid sort field '{request.SortBy}'.")
			};
		else if (request.SortDirection.ToLower() == "desc")
			query = request.SortBy.ToLower() switch
			{
				"title" => query.OrderByDescending(m => m.Title),
				"durationMinutes" => query.OrderByDescending(m => m.DurationMinutes),
				"producer" => query.OrderByDescending(m => m.Producer),
				"age" => query.OrderByDescending(m => m.AgeLimit),
				"release" => query.OrderByDescending(m => m.ReleaseDate),
				_ => throw new InvalidOperationException($"Invalid sort field '{request.SortBy}'.")
			};

		query = query.Skip((request.Offset - 1) * request.Limit)
			.Take(request.Limit);

		var moviesEntities = await unitOfWork.MoviesRepository.ToListAsync(query, cancellationToken);

		var movies = mapper.Map<IList<MovieModel>>(moviesEntities);

		await redisCacheService.SetValueAsync(cacheKey, movies, TimeSpan.FromMinutes(10));

		return new PaginationWrapperDto<MovieModel>(
			movies,
			request.Limit,
			request.Offset,
			totalMovies);
	}
}