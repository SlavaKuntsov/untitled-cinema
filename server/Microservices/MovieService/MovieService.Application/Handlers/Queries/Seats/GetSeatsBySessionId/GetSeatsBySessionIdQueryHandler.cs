using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;

public class GetSeatsBySessionIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSeatsBySessionIdQuery, IList<SeatModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<SeatModel>> Handle(GetSeatsBySessionIdQuery request, CancellationToken cancellationToken)
	{
		var seats = await _unitOfWork.SeatsRepository.GetBySessionIdAsync(request.Id, cancellationToken);

		return _mapper.Map< IList<SeatModel>>(seats);
	}
}