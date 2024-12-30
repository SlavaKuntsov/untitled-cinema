using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Commands.Users.UserRegistration;

public partial class UserRegistrationCommand(
    string email,
    string password,
    string firstName,
    string lastName,
    string dateOfBirth) : IRequest<AuthDto>
{
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string DateOfBirth { get; private set; } = dateOfBirth;
}