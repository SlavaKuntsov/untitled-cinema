using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatTypesByHallId;

public class GetSeatTypesByHallIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSeatTypesByHallIdQuery, IList<SeatTypeModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<SeatTypeModel>> Handle(GetSeatTypesByHallIdQuery request, CancellationToken cancellationToken)
	{
		var seatTypes = await _unitOfWork.SeatsRepository.GetTypeByHallIdAsync(
			request.HallId,
			cancellationToken);

		return _mapper.Map<IList<SeatTypeModel>>(seatTypes);
	}
}