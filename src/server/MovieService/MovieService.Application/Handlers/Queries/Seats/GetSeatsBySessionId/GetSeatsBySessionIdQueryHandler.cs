using MapsterMapper;
using MediatR;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatsBySessionId;

public class GetSeatsBySessionIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSeatsBySessionIdQuery, IList<SeatModel>>
{
	public async Task<IList<SeatModel>> Handle(
		GetSeatsBySessionIdQuery request,
		CancellationToken cancellationToken)
	{
		var seats = await unitOfWork.SeatsRepository.GetBySessionIdAsync(
				request.SessionId,
				cancellationToken);

		return mapper.Map<IList<SeatModel>>(seats);
	}
}