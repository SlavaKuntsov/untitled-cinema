using Domain.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

public class UpdateSeatCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateSeatCommand, SeatModel>
{
	public async Task<SeatModel> Handle(UpdateSeatCommand request, CancellationToken cancellationToken)
	{
		var existSeat = await unitOfWork.Repository<SeatEntity>().GetAsync(request.Id, cancellationToken)
						?? throw new NotFoundException($"Seat with id '{request.Id}' doesn't exists");

		request.Adapt(existSeat);

		unitOfWork.Repository<SeatEntity>().Update(existSeat);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return mapper.Map<SeatModel>(existSeat);
	}
}