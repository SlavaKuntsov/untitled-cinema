using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.UpdateHall;

public class UpdateHallCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateHallCommand, HallModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<HallModel> Handle(UpdateHallCommand request, CancellationToken cancellationToken)
	{
		var existHall = await _unitOfWork.HallsRepository.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");

		request.Adapt(existHall);

		_unitOfWork.HallsRepository.Update(existHall, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return _mapper.Map<HallModel>(existHall);
	}
}