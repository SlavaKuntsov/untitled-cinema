using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Enums;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Extensions;
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
		var existHallEntity = await _unitOfWork.Repository<HallEntity>().GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");
		request.Adapt(existHallEntity);

		existHallEntity.Seats = null;

		var existHallModel = _mapper.Map<HallModel>(existHallEntity);
		var seatModels = new List<SeatModel>();

		for (int row = 0; row < existHallModel.SeatsArray.Length; row++)
		{
			for (int column = 0; column < existHallModel.SeatsArray[row].Length; column++)
			{
				var seatType = (SeatType)existHallModel.SeatsArray[row][column];

				if (seatType == SeatType.None)
					continue;

				var seatTypeDescription = seatType.GetDescription();
				var seatTypeEntity = await _unitOfWork.SeatsRepository
					.GetTypeAsync(seatTypeDescription, cancellationToken)
					?? throw new NotFoundException($"Seat type with name '{seatTypeDescription}' doesn't exists");

				var seat = new SeatModel(
						Guid.NewGuid(),
						existHallModel.Id,
						seatTypeEntity.Id,
						row + 1,
						column + 1
					);

				seatModels.Add(seat);
			}
		}

		_unitOfWork.Repository<HallEntity>().Update(existHallEntity);
		_unitOfWork.SeatsRepository.DeleteByHallId(existHallEntity.Id);
		await _unitOfWork.SeatsRepository
			.CreateRangeAsync(_mapper.Map<IList<SeatEntity>>(seatModels), cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return existHallModel;
	}
}