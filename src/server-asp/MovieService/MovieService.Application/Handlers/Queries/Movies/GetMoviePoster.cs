using Domain.Exceptions;
using Extensions.Enums;
using MediatR;
using Minios.Enums;
using Minios.Services;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Queries.Movies;

public record GetMoviePoster(Guid MovieId) : IRequest<string>;

public class GetMoviePosterQueryHandler(
	IMinioService minioService,
	IUnitOfWork unitOfWork) : IRequestHandler<GetMoviePoster, string>
{
	public async Task<string> Handle(
		GetMoviePoster request,
		CancellationToken cancellationToken)
	{
		var movie = await unitOfWork.MoviesRepository.GetAsync(
						request.MovieId,
						cancellationToken)
					?? throw new NotFoundException($"Movie with id '{request}' not found.");

		if (movie.Poster is null)
			throw new UnprocessableContentException($"Movie with id '{request}' doesn't have a poster.");

		var poster = await minioService.GetPresignedUrlAsync(
			Buckets.Poster.GetDescription(),
			movie.Poster);

		return poster;
	}
}