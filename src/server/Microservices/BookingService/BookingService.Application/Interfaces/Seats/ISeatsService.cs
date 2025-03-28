using BookingService.Application.DTOs;

namespace UserService.Application.Interfaces.Notification;

public interface ISeatsService
{
	Task NotifySeatChangedAsync(UpdatedSeatDTO seat, CancellationToken cancellationToken);
}