using Brokers.Models.DTOs;

namespace UserService.Application.Interfaces.Notification;

public interface INotificationService
{
	Task SendAsync(NotificationDto notification, CancellationToken cancellationToken);
}