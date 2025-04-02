using Domain.Exceptions;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using Redis.Service;

namespace MovieService.Application.Handlers.Commands.Sessions.DeleteSession;

public class DeleteSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService) : IRequestHandler<DeleteSessionCommand>
{
	public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
	{
		var movie = await unitOfWork.Repository<SessionEntity>().GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		unitOfWork.Repository<SessionEntity>().Delete(movie);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		await redisCacheService.RemoveValuesByPatternAsync("movies_*");
	}
}