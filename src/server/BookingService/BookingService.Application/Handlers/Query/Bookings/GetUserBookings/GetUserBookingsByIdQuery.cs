using BookingService.Domain.Entities;

using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetBookingsByUserId;

public class GetUserBookingsByIdQuery(Guid userId) : IRequest<IList<BookingEntity>>
{
	public Guid UserId { get; private set; } = userId;
}