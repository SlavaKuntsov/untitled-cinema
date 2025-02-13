using MediatR;

using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

public record struct UpdateUserCommand(
	Guid? Id,
	string FirstName,
	string LastName,
	string DateOfBirth) : IRequest<UserModel>;