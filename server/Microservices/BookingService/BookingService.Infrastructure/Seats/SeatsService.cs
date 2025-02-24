using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Seats;

//public class SeatsHub(
//	IHubContext<NotificationHub> hubContext,
//	ILogger<NotificationService> logger) : INotificationService
//{
//	private readonly IHubContext<NotificationHub> _hubContext = hubContext;

//	public async Task SendAsync(NotificationModel notification, CancellationToken cancellationToken)
//	{
//		var connections = NotificationHub.GetConnections(notification.UserId);

//		foreach (var connectionId in connections)
//		{
//			logger.LogError("coonect count: " + connections.Count());

//			await _hubContext.Clients.Client(connectionId).SendAsync(
//				"ReceiveNotification",
//				notification,
//				cancellationToken);
//		}
//	}
//}