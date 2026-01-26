using Domain.Enums;

namespace UserService.Domain.Entities;

public class UserEntity
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public Role Role { get; set; } = Role.User;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string DateOfBirth { get; set; }
	public decimal Balance { get; set; } = 0;

	public virtual RefreshTokenEntity RefreshToken { get; set; } = null!;
	public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];
}