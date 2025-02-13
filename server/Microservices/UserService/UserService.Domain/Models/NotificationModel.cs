namespace UserService.Domain.Models;

public class NotificationModel
{
	public Guid Id { get; set; }
	public string Message { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public Guid UserId { get; private set; }

	public NotificationModel() { }

	public NotificationModel(
		Guid id,
		Guid userId,
		string message,
		DateTime createdAt)
	{
		Id = id;
		UserId = userId;
		Message = message;
		CreatedAt = createdAt;
	}
}