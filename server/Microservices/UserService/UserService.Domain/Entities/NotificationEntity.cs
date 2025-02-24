namespace UserService.Domain.Entities;

public class NotificationEntity
{
	public Guid Id { get; set; }
	public string Message { get; set; } = string.Empty;
	public bool IsDeleted { get; set; } = false;
	public DateTime CreatedAt { get; set; }

	public Guid UserId { get; set; }
	public virtual UserEntity User { get; set; } = null!;
}