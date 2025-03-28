using BookingService.Application.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Notification;

namespace BookingService.Infrastructure.Seats;

public class SeatsService(
	IHubContext<SeatsHub> hubContext,
	ILogger<SeatsService> logger) : ISeatsService
{
	public async Task NotifySeatChangedAsync(UpdatedSeatDTO seat, CancellationToken cancellationToken)
	{
		await hubContext.Clients.Group($"session-{seat.SessionId}")
			.SendAsync(
				"SeatChanged", 
				seat, 
				cancellationToken);
	}
}