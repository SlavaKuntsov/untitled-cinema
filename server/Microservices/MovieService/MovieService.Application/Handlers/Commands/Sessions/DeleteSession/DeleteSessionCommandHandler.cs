using MediatR;

using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Sessions.DeleteSession;

public class DeleteSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService) : IRequestHandler<DeleteSessionCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;

	public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
	{
		var movie = await _unitOfWork.Repository<SessionEntity>().GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		_unitOfWork.Repository<SessionEntity>().Delete(movie);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		await _redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return;
	}
}