using Domain.Exceptions;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Days.DeleteDay;

public class DeleteDayCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteDayCommand>
{
	public async Task Handle(DeleteDayCommand request, CancellationToken cancellationToken)
	{
		var hall = await unitOfWork.Repository<DayEntity>().GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Day with id {request.Id} doesn't exists");

		unitOfWork.Repository<DayEntity>().Delete(hall);

		await unitOfWork.SaveChangesAsync(cancellationToken);
	}
}