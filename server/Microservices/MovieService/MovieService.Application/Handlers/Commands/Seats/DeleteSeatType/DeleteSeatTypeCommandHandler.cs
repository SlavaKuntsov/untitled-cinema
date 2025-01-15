using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Seats.DeleteSeatType;

public class DeleteSeatTypeCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<DeleteSeatTypeCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteSeatTypeCommand request, CancellationToken cancellationToken)
	{
		var seatType = await _unitOfWork.SeatsRepository.GetTypeAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Seat type with name '{request.Id}' doesn't exists");

		_unitOfWork.SeatsRepository.Delete(seatType);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}