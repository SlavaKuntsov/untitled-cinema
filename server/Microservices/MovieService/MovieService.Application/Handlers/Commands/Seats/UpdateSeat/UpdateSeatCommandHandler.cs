using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

public class UpdateSeatCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateSeatCommand, SeatModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<SeatModel> Handle(UpdateSeatCommand request, CancellationToken cancellationToken)
	{
		var existSeat = await _unitOfWork.Repository<SeatEntity>().GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Seat with id '{request.Id}' doesn't exists");

		request.Adapt(existSeat);

		_unitOfWork.Repository<SeatEntity>().Update(existSeat);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return _mapper.Map<SeatModel>(existSeat);
	}
}