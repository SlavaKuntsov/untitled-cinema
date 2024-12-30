using System.Globalization;

using Mapster;

using MediatR;

using UserService.Domain;
using UserService.Domain.Entities;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

public partial class UpdateUserCommand(
    Guid id,
    string firstName,
    string lastName,
    string dateOfBirth) : IRequest<UserModel>
{
    public Guid Id { get; private set; } = id;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string DateOfBirth { get; private set; } = dateOfBirth;
}