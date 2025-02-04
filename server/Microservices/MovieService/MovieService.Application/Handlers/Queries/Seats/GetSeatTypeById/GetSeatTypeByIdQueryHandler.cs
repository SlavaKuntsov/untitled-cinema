using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatTypeById;

public class GetSeatByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSeatTypeByIdQuery, SeatTypeModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<SeatTypeModel> Handle(GetSeatTypeByIdQuery request, CancellationToken cancellationToken)
	{
		var seatType = await _unitOfWork.Repository<SeatTypeEntity>().GetAsync(request.Id, cancellationToken);

		return _mapper.Map<SeatTypeModel>(seatType);
	}
}