using UserService.Domain.Models;

namespace UserService.Application.Interfaces.Notification;

public interface INotificationService
{
	Task SendAsync(NotificationModel notification, CancellationToken cancellationToken);
}