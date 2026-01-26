using Domain.Exceptions;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Halls.DeleteHall;

public class DeleteHallCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteHallCommand>
{
	public async Task Handle(DeleteHallCommand request, CancellationToken cancellationToken)
	{
		var hall = await unitOfWork.Repository<HallEntity>().GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");

		unitOfWork.Repository<HallEntity>().Delete(hall);
		unitOfWork.SeatsRepository.DeleteBySessionId(request.Id);

		await unitOfWork.SaveChangesAsync(cancellationToken);
	}
}