using BookingService.Domain.Entities;

using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetAllBookings;

public class GetAllBookingsQuery() : IRequest<IList<BookingEntity>>
{
}