using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

public record struct UpdateUserCommand(
	Guid? Id,
	string FirstName,
	string LastName,
	string DateOfBirth) : IRequest<UserDto>;