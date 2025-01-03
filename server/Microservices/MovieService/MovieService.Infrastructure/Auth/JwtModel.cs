namespace MovieService.Infrastructure.Auth;

public class JwtModel
{
	public int ExpiresHours { get; set; }

	public string SecretKey { get; set; }

	public int AccessTokenExpirationMinutes { get; set; }

	public int RefreshTokenExpirationDays { get; set; }
}
