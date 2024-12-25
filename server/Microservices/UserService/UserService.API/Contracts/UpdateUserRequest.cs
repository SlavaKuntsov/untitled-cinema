using UserService.Application.Interfaces.Auth;

namespace UserService.API.Contracts;

public record UpdateUserRequest(
	Guid Id,
	string Firstname,
	string Lastname,
	string DateOfBirth);
