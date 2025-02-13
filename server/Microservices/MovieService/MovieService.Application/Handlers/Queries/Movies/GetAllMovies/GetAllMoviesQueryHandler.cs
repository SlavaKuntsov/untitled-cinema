using MapsterMapper;

using MediatR;

using MovieService.Application.DTOs;
using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public class GetAllMoviesQueryHandler(
	IUnitOfWork unitOfWork,
	//IRedisCacheService redisCacheService,
	IMapper mapper
	) : IRequestHandler<GetAllMoviesQuery, PaginationWrapperDto<MovieModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;
	//private readonly IRedisCacheService _redisCacheService = redisCacheService;

	public async Task<PaginationWrapperDto<MovieModel>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
	{
		var totalMovies = await _unitOfWork.MoviesRepository.GetCount();

		string cacheKey = @$"movies_
			{request.Filter}_
			{request.FilterValue}_
			{request.SortBy}_
			{request.SortDirection}_
			{request.Offset}_
			{request.Limit}";

		//var cachedMovies = await _redisCacheService.GetValueAsync<IList<MovieModel>>(cacheKey);

		//if (cachedMovies != null)
		//{
		//	return new PaginationWrapperDto<MovieModel>(
		//		cachedMovies,
		//		request.Limit,
		//		request.Offset,
		//		totalMovies);
		//}

		var query = _unitOfWork.MoviesRepository.Get();

		if (!string.IsNullOrWhiteSpace(request.Filter) && !string.IsNullOrWhiteSpace(request.FilterValue))
		{
			query = request.Filter!.ToLower() switch
			{
				"genre" => _unitOfWork.MoviesRepository.FilterByGenre(query, request.FilterValue.ToLower()),
				"producer" => query.Where(m => m.Producer.ToLower().Contains(request.FilterValue.ToLower())),
				_ => throw new InvalidOperationException($"Invalid filter field '{request.Filter}'.")
			};
		}

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

		//await _redisCacheService.SetValueAsync(cacheKey, movieModels, TimeSpan.FromMinutes(10));

		return new PaginationWrapperDto<MovieModel>(
			movieModels,
			request.Limit,
			request.Offset,
			totalMovies);
	}
}