using UserService.Domain.Enums;

namespace UserService.Domain.Models.Users;

public class UserModel
{
	public Guid Id { get; private set; }
	public string Email { get; private set; } = string.Empty;
	public string Password { get; private set; } = string.Empty;
	public Role Role { get; private set; }
	public string FirstName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = string.Empty;
	public DateTime DateOfBirth { get; private set; }

	public UserModel(
		Guid id,
		string email,
		string password,
		Role role,
		string? firstName = null,
		string? lastName = null,
		DateTime? dateOfBirth = null)
	{
		Id = id;
		Email = email;
		Password = password;
		Role = role;
		FirstName = firstName ?? string.Empty;
		LastName = lastName ?? string.Empty;
		DateOfBirth = dateOfBirth ?? DateTime.MinValue;
	}
}