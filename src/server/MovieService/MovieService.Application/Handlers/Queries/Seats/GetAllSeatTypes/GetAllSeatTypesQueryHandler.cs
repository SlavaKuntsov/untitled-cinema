using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetAllSeatTypes;

public class GetAllSeatTypesQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllSeatTypesQuery, IList<SeatTypeModel>>
{
	public async Task<IList<SeatTypeModel>> Handle(GetAllSeatTypesQuery request, CancellationToken cancellationToken)
	{
		var seatTypes = await unitOfWork.Repository<SeatTypeEntity>().GetAsync(cancellationToken);

		return mapper.Map<IList<SeatTypeModel>>(seatTypes);
	}
}