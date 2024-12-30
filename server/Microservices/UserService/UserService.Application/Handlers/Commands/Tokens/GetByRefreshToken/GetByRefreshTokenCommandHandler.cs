using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Tokens.RefreshToken;

public class GetByRefreshTokenCommandHandler(IUsersRepository usersRepository, IJwt jwt) : IRequestHandler<GetByRefreshTokenCommand, UserRoleDto>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IJwt _jwt = jwt;

	public async Task<UserRoleDto> Handle(GetByRefreshTokenCommand request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(request.RefreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var userId = await _jwt.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

		if (userId == Guid.Empty)
			throw new InvalidTokenException("Invalid refresh token");

		var role = await _usersRepository.GetRoleByIdAsync(userId, cancellationToken)
				?? throw new NotFoundException("User not found");

		return new UserRoleDto(userId, role);
	}
}