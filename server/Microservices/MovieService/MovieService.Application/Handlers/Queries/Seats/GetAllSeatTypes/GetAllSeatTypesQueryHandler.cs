using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatTypes;

public class GetAllSeatTypesQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllSeatTypesQuery, IList<SeatTypeModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<SeatTypeModel>> Handle(GetAllSeatTypesQuery request, CancellationToken cancellationToken)
	{
		var seatTypes = await _unitOfWork.Repository<SeatTypeEntity>().GetAsync(cancellationToken);

		return _mapper.Map<IList<SeatTypeModel>>(seatTypes);
	}
}