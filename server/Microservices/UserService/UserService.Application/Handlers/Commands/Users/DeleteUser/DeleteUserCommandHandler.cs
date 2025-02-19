using MediatR;

using UserService.Application.Interfaces.Caching;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService) : IRequestHandler<DeleteUserCommand>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;

	public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var (userId, role, refreshTokenId) = await _usersRepository.GetIdWithRoleAndTokenAsync(
			request.Id,
			cancellationToken);

		if (userId is null)
			throw new NotFoundException($"User with id {request.Id} doesn't exists");

		if (refreshTokenId is null)
			throw new NotFoundException($"Refresh Token for user with id {request.Id} not found");

		if (role is Role.User)
		{
			_usersRepository.Delete(userId.Value, refreshTokenId.Value);
		}
		else if (role is Role.Admin)
		{
			var admins = await _usersRepository.GetByRole(Role.Admin, cancellationToken);

			if (admins.Count == 1)
				throw new UnprocessableContentException("Cannot delete the last Admin");

			_usersRepository.Delete(userId.Value, refreshTokenId.Value);
		}

		await _redisCacheService.RemoveValuesByPatternAsync("users_*");
	}
}