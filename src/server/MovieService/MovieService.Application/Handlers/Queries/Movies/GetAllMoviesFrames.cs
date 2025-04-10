using MediatR;
using Minios.Services;
using MovieService.Application.DTOs;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Queries.Movies;

public sealed record GetAllMoviesFramesQuery : IRequest<IList<MovieFrameDto>>;

public sealed class GetAllMoviesFramesQueryHandler(
	IMinioService minioService,
	IUnitOfWork unitOfWork) : IRequestHandler<GetAllMoviesFramesQuery, IList<MovieFrameDto>>
{
	public async Task<IList<MovieFrameDto>> Handle(
		GetAllMoviesFramesQuery request,
		CancellationToken cancellationToken)
	{
		var frames = await unitOfWork.Repository<MovieFrameEntity>()
			.GetAsync(cancellationToken);

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