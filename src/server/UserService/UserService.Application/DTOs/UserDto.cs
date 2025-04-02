namespace UserService.Application.DTOs;

public record UserDto(
	Guid Id,
	string Email,
	string Role,
	string FirstName,
	string LastName,
	string DateOfBirth,
	decimal Balance);