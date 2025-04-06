using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using Extensions.Enums;
using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetUserBookings;

public record GetUserBookingsByIdQuery(
	Guid UserId,
	bool Processed) : IRequest<IList<BookingEntity>>;

public class GetUserBookingsByIdQueryHandler(IBookingsRepository bookingsRepository)
	: IRequestHandler<GetUserBookingsByIdQuery, IList<BookingEntity>>
{
	public async Task<IList<BookingEntity>> Handle(
		GetUserBookingsByIdQuery request,
		CancellationToken cancellationToken)
	{
		if (!request.Processed)
			return await bookingsRepository.GetAsync(
				b => b.UserId == request.UserId,
				cancellationToken);
		
		return await bookingsRepository.GetAsync(
			b => b.UserId == request.UserId && b.Status != BookingStatus.Cancelled.GetDescription(),
			cancellationToken);
	}
}