using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Halls.DeleteHall;

public class DeleteHallCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<DeleteHallCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteHallCommand request, CancellationToken cancellationToken)
	{
		var hall = await _unitOfWork.HallsRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");

		_unitOfWork.HallsRepository.Delete(hall);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}