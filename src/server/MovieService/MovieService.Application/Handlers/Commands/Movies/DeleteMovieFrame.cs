using Domain.Exceptions;
using MediatR;
using Minios.Services;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies;

public sealed record DeleteMovieFrameCommand(Guid Id) : IRequest;

public sealed class DeleteMovieFrameCommandHandler(
	IMinioService minioService,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeleteMovieFrameCommand>
{
	public async Task Handle(DeleteMovieFrameCommand request, CancellationToken cancellationToken)
	{
		var frameToDelete = await unitOfWork.Repository<MovieFrameEntity>()
								.GetAsync(request.Id, cancellationToken)
							?? throw new NotFoundException(
								$"Movie Frame with id {request.Id} doesn't exists");

		unitOfWork.Repository<MovieFrameEntity>().Delete(frameToDelete);

		var framesToUpdate = await unitOfWork.MoviesRepository
			.GetFramesAsync(frameToDelete.MovieId, cancellationToken);

		var subsequentFrames = framesToUpdate
			.Where(f => f.Order > frameToDelete.Order)
			.OrderBy(f => f.Order)
			.ToList();

		foreach (var frame in subsequentFrames)
		{
			frame.Order -= 1;
			unitOfWork.Repository<MovieFrameEntity>().Update(frame);
		}

		await minioService.RemoveFileAsync(null, frameToDelete.FrameName);

		await unitOfWork.SaveChangesAsync(cancellationToken);
	}
}