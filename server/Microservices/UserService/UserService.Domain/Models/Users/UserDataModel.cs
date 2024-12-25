namespace UserService.Domain.Models.Users;

public class UserDataModel
{
	public string FirstName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = string.Empty;
	public DateTime DateOfBirth { get; private set; }

	public UserDataModel(string firstName, string lastName, DateTime dateOfBirth)
	{
		FirstName = firstName;
		LastName = lastName;
		DateOfBirth = dateOfBirth;
	}
}