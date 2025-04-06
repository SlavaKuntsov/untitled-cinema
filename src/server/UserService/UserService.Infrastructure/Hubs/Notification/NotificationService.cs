using Brokers.Models.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Notification;

namespace UserService.Infrastructure.Hubs.Notification;

public class NotificationService(
	IHubContext<NotificationHub> hubContext,
	ILogger<NotificationService> logger) : INotificationService
{
	public async Task SendAsync(NotificationDto notification, CancellationToken cancellationToken)
	{
		var connections = NotificationHub.GetConnections(notification.UserId);

		foreach (var connectionId in connections)
		{
			logger.LogError("Connect count: {Count}", connections.Count());

			await hubContext.Clients.Client(connectionId)
				.SendAsync(
					"ReceiveNotification",
					notification,
					cancellationToken);
		}
	}
}