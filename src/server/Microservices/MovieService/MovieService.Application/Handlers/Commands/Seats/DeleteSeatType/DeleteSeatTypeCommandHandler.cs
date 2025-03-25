using Domain.Exceptions;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Seats.DeleteSeatType;

public class DeleteSeatTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSeatTypeCommand>
{
	public async Task Handle(DeleteSeatTypeCommand request, CancellationToken cancellationToken)
	{
		var seatType = await unitOfWork.Repository<SeatTypeEntity>().GetAsync(request.Id, cancellationToken)
						?? throw new NotFoundException($"Seat type with name '{request.Id}' doesn't exists");

		unitOfWork.Repository<SeatTypeEntity>().Delete(seatType);

		await unitOfWork.SaveChangesAsync(cancellationToken);
	}
}