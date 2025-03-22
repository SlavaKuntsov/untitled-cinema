namespace UserService.Infrastructure.Auth;

public class JwtModel
{
	public string SecretKey { get; set; }
	public int AccessTokenExpirationMinutes { get; set; }
	public int RefreshTokenExpirationDays { get; set; }
}
