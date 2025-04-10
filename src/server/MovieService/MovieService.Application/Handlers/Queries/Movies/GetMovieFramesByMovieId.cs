using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies;

public sealed record GetMovieFramesByMovieIdQuery(Guid Id) : IRequest<IList<MovieFrameModel>>;

public sealed class GetMovieFramesByMovieIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetMovieFramesByMovieIdQuery, IList<MovieFrameModel>>
{
	public async Task<IList<MovieFrameModel>> Handle(
		GetMovieFramesByMovieIdQuery request,
		CancellationToken cancellationToken)
	{
		var frames = await unitOfWork.MoviesRepository
						.GetFramesAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Movie Frame with id '{request}' not found.");

		var orderedFrames = frames.OrderBy(f => f.Order);

		return mapper.Map<IList<MovieFrameModel>>(orderedFrames);
	}
}