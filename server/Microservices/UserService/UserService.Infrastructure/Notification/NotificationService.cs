using Microsoft.AspNetCore.SignalR;

using UserService.Application.Interfaces.Notification;

namespace UserService.Infrastructure.Notification;

public class NotificationService : INotificationService
{
	private readonly IHubContext<NotificationHub> _hubContext;

	public NotificationService(IHubContext<NotificationHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task SendAsync(Guid userId, string message)
	{
		var connections = NotificationHub.GetConnections(userId);

		foreach (var connectionId in connections)
		{
			await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
		}
	}
}