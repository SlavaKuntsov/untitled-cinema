namespace UserService.API.Contracts;

public record CreateLoginRequest(
	string Email,
	string Password);
