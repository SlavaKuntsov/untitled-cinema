namespace Extensions.Authorization;

public class JwtOptions
{
	public string SecretKey { get; set; }
	public int AccessTokenExpirationMinutes { get; set; }
	public int RefreshTokenExpirationDays { get; set; }
}