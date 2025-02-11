using MediatR;

using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

//public partial class UpdateUserCommand(
//	Guid id,
//	string firstName,
//	string lastName,
//	string dateOfBirth) : IRequest<UserModel>
//{
//	public Guid Id { get; private set; } = id;
//	public string FirstName { get; private set; } = firstName;
//	public string LastName { get; private set; } = lastName;
//	public string DateOfBirth { get; private set; } = dateOfBirth;
//}

public record UpdateUserCommand(Guid Id,
	string FirstName,
	string LastName,
	string DateOfBirth) : IRequest<UserModel>;