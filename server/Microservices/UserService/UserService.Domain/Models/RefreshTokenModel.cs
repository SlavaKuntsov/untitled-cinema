using UserService.Domain.Enums;

namespace UserService.Domain.Models;

public class RefreshTokenModel
{
	public Guid Id { get; private set; }
	public string Token { get; private set; } = string.Empty;
	public DateTime ExpiresAt { get; private set; }
	public bool IsRevoked { get; private set; } = false;
	public DateTime CreatedAt { get; private set; }
	public Guid? UserId { get; private set; }

	public RefreshTokenModel() { }

	public RefreshTokenModel(Guid userId, Role role, string token, int refreshTokenExpirationDays)
	{
		Id = Guid.NewGuid();
		Token = token;
		ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(refreshTokenExpirationDays));
		CreatedAt = DateTime.UtcNow;
		IsRevoked = false;
		UserId = userId;
	}
}
