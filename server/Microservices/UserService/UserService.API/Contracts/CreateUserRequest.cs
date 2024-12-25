using UserService.Application.Interfaces.Auth;

namespace UserService.API.Contracts;

public record CreateUserRequest(
	string Email,
	string Password,
	string PasswordConfirmation,
	//bool IsAdmin,
	string Role,
	string Firstname,
	string Lastname,
	string DateOfBirth) : IHasRole;
