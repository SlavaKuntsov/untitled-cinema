namespace UserService.Application.DTOs;

public class AuthDto
{
	public string AccessToken { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
}