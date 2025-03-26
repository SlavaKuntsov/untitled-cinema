using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Commands.Users.UserRegistration;

public record UserRegistrationCommand(
	string Email,
	string Password,
	string FirstName,
	string LastName,
	string DateOfBirth) : IRequest<AuthDto>;