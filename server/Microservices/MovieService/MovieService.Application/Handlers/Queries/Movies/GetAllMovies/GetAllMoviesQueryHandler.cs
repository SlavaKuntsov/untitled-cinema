using MapsterMapper;

using MediatR;

using MovieService.Application.DTOs;
using MovieService.Application.Extensions;
using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public class GetAllMoviesQueryHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper
	) : IRequestHandler<GetAllMoviesQuery, PaginationWrapperDto<MovieModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;

	public async Task<PaginationWrapperDto<MovieModel>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
	{
		if (request.Filters.Length != request.FilterValues.Length)
			throw new InvalidOperationException("The number of Filters and FilterValues must be the same.");

		var filters = request.Filters
			.Zip(request.FilterValues, (field, value) => new FilterDto(field, value))
			.ToList();

		string cacheKey = @$"movies_{
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

		var cachedMovies = await _redisCacheService.GetValueAsync<IList<MovieModel>?>(cacheKey);

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
			if (!request.Date.DateFormatTryParse(out DateTime parsedDate))
				throw new BadRequestException("Invalid date format.");

			query = _unitOfWork.MoviesRepository.Get(parsedDate);
		}
		else
		{
			query = _unitOfWork.MoviesRepository.Get();
		}

		foreach (var filter in filters)
		{
			if (!string.IsNullOrWhiteSpace(filter.Field) && !string.IsNullOrWhiteSpace(filter.Value))
			{
				if (filter.Field.ToLower() == "genre")
				{
					var genreFilters = filters
						.Where(f => f.Field.ToLower() == "genre")
						.Select(f => f.Value.ToLower())
						.ToList();

					query = _unitOfWork.MoviesRepository.FilterByGenre(query, genreFilters);
					break; 
				}
				else
				{
					query = filter.Field.ToLower() switch
					{
						"producer" => query.Where(m =>
							m.Producer.ToLower().Contains(filter.Value.ToLower())),
						"title" => query.Where(m =>
							m.Title.ToLower().Contains(filter.Value.ToLower())),
						_ => throw new InvalidOperationException($"Invalid filter field '{filter.Field}'.")
					};
				}
			}
		}

		totalMovies = await _unitOfWork.MoviesRepository.GetCount(query);

		if (request.SortDirection.ToLower() == "asc")
		{
			query = request.SortBy.ToLower() switch
			{
				"title" => query.OrderBy(m => m.Title),
				"durationMinutes" => query.OrderBy(m => m.DurationMinutes),
				"producer" => query.OrderBy(m => m.Producer),
				"age" => query.OrderBy(m => m.AgeLimit),
				"release" => query.OrderBy(m => m.ReleaseDate),
				_ => throw new InvalidOperationException($"Invalid sort field '{request.SortBy}'.")
			};
		}
		else if (request.SortDirection.ToLower() == "desc")
		{
			query = request.SortBy.ToLower() switch
			{
				"title" => query.OrderByDescending(m => m.Title),
				"durationMinutes" => query.OrderByDescending(m => m.DurationMinutes),
				"producer" => query.OrderByDescending(m => m.Producer),
				"age" => query.OrderByDescending(m => m.AgeLimit),
				"release" => query.OrderByDescending(m => m.ReleaseDate),
				_ => throw new InvalidOperationException($"Invalid sort field '{request.SortBy}'.")
			};
		}

		query = query.Skip((request.Offset - 1) * request.Limit)
			.Take(request.Limit);

		var movies = await _unitOfWork.MoviesRepository.ToListAsync(query, cancellationToken);

		var movieModels = _mapper.Map<IList<MovieModel>>(movies);

		await _redisCacheService.SetValueAsync(cacheKey, movieModels, TimeSpan.FromMinutes(10));

		return new PaginationWrapperDto<MovieModel>(
			movieModels,
			request.Limit,
			request.Offset,
			totalMovies);
	}
}