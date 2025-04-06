using Domain.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.UpdateSeatType;

public class UpdateSeatTypeCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateSeatTypeCommand, SeatTypeModel>
{
	public async Task<SeatTypeModel> Handle(UpdateSeatTypeCommand request, CancellationToken cancellationToken)
	{
		var existSeatType = await unitOfWork.Repository<SeatTypeEntity>().GetAsync(request.Id, cancellationToken)
							?? throw new NotFoundException($"Seat type with id '{request.Id}' doesn't exists");

		request.Adapt(existSeatType);

		unitOfWork.Repository<SeatTypeEntity>().Update(existSeatType);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return mapper.Map<SeatTypeModel>(existSeatType);
	}
}