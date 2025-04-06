namespace UserService.Domain.Models;

public class RefreshTokenModel
{
	public Guid Id { get;  set; }
	public string Token { get;  set; } = string.Empty;
	public DateTime ExpiresAt { get;  set; }
	public bool IsRevoked { get;  set; } = false;
	public DateTime CreatedAt { get;  set; }
	public Guid? UserId { get;  set; }

	public RefreshTokenModel() { }

	public RefreshTokenModel(
		Guid userId,
		string token,
		int refreshTokenExpirationDays)
	{
		Id = Guid.NewGuid();
		Token = token;
		ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(refreshTokenExpirationDays));
		CreatedAt = DateTime.UtcNow;
		IsRevoked = false;
		UserId = userId;
	}
}