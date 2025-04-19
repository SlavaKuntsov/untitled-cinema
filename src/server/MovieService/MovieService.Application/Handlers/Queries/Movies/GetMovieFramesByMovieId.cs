using Domain.Exceptions;
using MediatR;
using Minios.Services;
using MovieService.Application.DTOs;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Queries.Movies;

public sealed record GetMovieFramesByMovieIdQuery(Guid Id) : IRequest<IList<MovieFrameDto>>;

public sealed class GetMovieFramesByMovieIdQueryHandler(
	IMinioService minioService,
	IUnitOfWork unitOfWork) : IRequestHandler<GetMovieFramesByMovieIdQuery, IList<MovieFrameDto>>
{
	public async Task<IList<MovieFrameDto>> Handle(
		GetMovieFramesByMovieIdQuery request,
		CancellationToken cancellationToken)
	{
		var frames = await unitOfWork.MoviesRepository
						.GetFramesAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Movie Frame with id '{request}' not found.");

		var result = new List<MovieFrameDto>(frames.Count);

		foreach (var frame in frames.OrderBy(f => f.Order))
		{
			var url = await minioService.GetPresignedUrlAsync(null, frame.FrameName);

			result.Add(
				new MovieFrameDto(
					frame.Id,
					frame.MovieId,
					frame.FrameName,
					url));
		}

		return result;
	}
}