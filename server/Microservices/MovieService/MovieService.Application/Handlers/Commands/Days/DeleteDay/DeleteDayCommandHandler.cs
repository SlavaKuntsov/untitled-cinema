using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Days.DeleteDay;

public class DeleteDayCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<DeleteDayCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteDayCommand request, CancellationToken cancellationToken)
	{
		var hall = await _unitOfWork.DaysRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Day with id {request.Id} doesn't exists");

		_unitOfWork.DaysRepository.Delete(hall);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}