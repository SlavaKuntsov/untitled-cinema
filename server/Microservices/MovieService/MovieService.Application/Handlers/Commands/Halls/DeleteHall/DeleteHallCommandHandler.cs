using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Halls.DeleteHall;

public class DeleteHallCommandHandler(
	IUnitOfWork unitOfWork) : IRequestHandler<DeleteHallCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task Handle(DeleteHallCommand request, CancellationToken cancellationToken)
	{
		var hall = await _unitOfWork.Repository<HallEntity>().GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");

		_unitOfWork.Repository<HallEntity>().Delete(hall);

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		_unitOfWork.SeatsRepository.DeleteBySessionId(request.Id);

		return;
	}
}