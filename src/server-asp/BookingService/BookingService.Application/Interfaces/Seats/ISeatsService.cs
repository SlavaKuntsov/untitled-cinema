using BookingService.Application.DTOs;

namespace BookingService.Application.Interfaces.Seats;

public interface ISeatsService
{
	Task NotifySeatChangedAsync(UpdatedSeatDTO seat, CancellationToken cancellationToken);
}