using BookingService.Domain.Exceptions;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Query.Seats.GetSeatsById;

public class GetSeatsByIdQueryHandler(
	ISessionSeatsRepository seatsRepository,
	IMapper mapper) : IRequestHandler<GetSeatsByIdQuery, SessionSeatsModel>
{
	private readonly ISessionSeatsRepository _seatsRepository = seatsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<SessionSeatsModel> Handle(GetSeatsByIdQuery request, CancellationToken cancellationToken)
	{
		var seat = await _seatsRepository.GetAsync(
			b => b.SessionId == request.SessionId,
			request.IsAvailable,
			cancellationToken)
			?? throw new NotFoundException("Session seat not found");

		return _mapper.Map<SessionSeatsModel>(seat);
	}
}