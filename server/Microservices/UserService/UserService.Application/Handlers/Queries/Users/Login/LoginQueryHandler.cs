using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.Login;

public class LoginQueryHandler(
	IUsersRepository usersRepository,
	IPasswordHash passwordHash) : IRequestHandler<LoginQuery, UserRoleDto>
{
	private readonly IPasswordHash _passwordHash = passwordHash;
	private readonly IUsersRepository _usersRepository = usersRepository;

	public async Task<UserRoleDto> Handle(LoginQuery request, CancellationToken cancellationToken)
	{
		var (userId, password, role) = await _usersRepository.GetIdWithRoleAndPasswordAsync(
			request.Email, 
			cancellationToken);

		if (userId is null)
			throw new NotFoundException($"User with email '{request.Email}' not found.");

		var isCorrectPassword = _passwordHash.Verify(request.Password, password!);

		if (!isCorrectPassword)
			throw new UnauthorizedAccessException("Incorrect password");

		return new UserRoleDto(userId!.Value, role!.Value);
	}
}