namespace UserService.Application.Interfaces.Notification;

public interface INotificationService
{
	//void AddConnection(string userId, string connectionId);
	//void RemoveConnection(string connectionId);
	Task SendAsync(Guid userId, string message);
}