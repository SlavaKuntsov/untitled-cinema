namespace UserService.Domain.Models;

public class NotificationModel
{
	public Guid Id { get; private set; }
	public Guid UserId { get; private set; }
	public string Message { get; private set; } = string.Empty;
	public bool IsDeleted { get; private set; } = false;
	public DateTime CreatedAt { get; private set; }

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