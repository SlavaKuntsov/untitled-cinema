using UserService.Domain.Enums;

namespace UserService.Persistence.Entities;

public class UserEntity
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public Role Role { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public DateTime DateOfBirth { get; set; }

	public virtual RefreshTokenEntity RefreshToken { get; set; } = null!;
}