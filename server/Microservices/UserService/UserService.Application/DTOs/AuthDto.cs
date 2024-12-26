namespace UserService.Application.DTOs;

public record AuthDto(
	string AccessToken,
	string RefreshToken);