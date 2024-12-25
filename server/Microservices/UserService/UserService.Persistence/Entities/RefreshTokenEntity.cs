namespace UserService.Persistence.Entities;

public class RefreshTokenEntity
{
	public Guid Id { get; set; }
	public Guid? UserId { get; set; }
	public string Token { get; set; } = string.Empty;
	public DateTime ExpiresAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool IsRevoked { get; set; }

	public virtual UserEntity User { get; set; }
}