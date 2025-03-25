using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.CreateSeatType;

public class CreateSeatTypeCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateSeatTypeCommand, Guid>
{
	public async Task<Guid> Handle(CreateSeatTypeCommand request, CancellationToken cancellationToken)
	{
		var existSeatType = await unitOfWork.SeatsRepository
			.GetTypeAsync(request.Name, cancellationToken);

		if (existSeatType is not null)
			throw new AlreadyExistsException($"Seat type with name {request.Name} already exist.");

		var seatType = new SeatTypeModel(
			Guid.NewGuid(),
			request.Name,
			request.PriceModifier);

		await unitOfWork.Repository<SeatTypeEntity>()
			.CreateAsync(mapper.Map<SeatTypeEntity>(seatType), cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return seatType.Id;
	}
}