using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Query.Seats.GetSeats;

public class GetSeatsByIdQueryHandler(
	ISessionSeatsRepository seatsRepository,
	IMapper mapper) : IRequestHandler<GetSeatsByIdQuery, IList<SessionSeatsModel>>
{
	private readonly ISessionSeatsRepository _seatsRepository = seatsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<SessionSeatsModel>> Handle(GetSeatsByIdQuery request, CancellationToken cancellationToken)
	{
		var seats = await _seatsRepository.GetAsync(
			b => b.SessionId == request.SessionId,
			request.IsAvailable,
			cancellationToken);

		return _mapper.Map<IList<SessionSeatsModel>>(seats);
	}
}