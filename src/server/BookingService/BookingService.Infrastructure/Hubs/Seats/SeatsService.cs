using BookingService.Application.DTOs;
using BookingService.Application.Interfaces.Seats;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Hubs.Seats;

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
		
		logger.LogInformation("NotifySeatChangedAsync called for {SessionId}", seat.SessionId);
	}
}