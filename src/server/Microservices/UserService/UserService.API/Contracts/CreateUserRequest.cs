namespace UserService.API.Contracts;

public record CreateUserRequest(
	string Email,
	string Password,
	string PasswordConfirmation,
	string Firstname,
	string Lastname,
	string DateOfBirth);