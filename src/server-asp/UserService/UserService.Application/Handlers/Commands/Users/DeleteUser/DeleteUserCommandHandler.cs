using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Redis.Services;
using UserService.Application.Data;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IDBContext dbContext) : IRequestHandler<DeleteUserCommand>
{
	public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var (userId, role, refreshTokenId) = await usersRepository.GetIdWithRoleAndTokenAsync(
			request.Id,
			cancellationToken);

		if (userId is null)
			throw new NotFoundException($"User with id {request.Id} doesn't exists");

		if (refreshTokenId is null)
			throw new NotFoundException($"Refresh Token for user with id {request.Id} not found");

		if (role is Role.User)
		{
			usersRepository.Delete(userId.Value, refreshTokenId.Value);
		}
		else if (role is Role.Admin)
		{
			var admins = await usersRepository.GetByRole(Role.Admin, cancellationToken);

			if (admins.Count == 1)
				throw new UnprocessableContentException("Cannot delete the last Admin");

			usersRepository.Delete(userId.Value, refreshTokenId.Value);
		}

		await redisCacheService.RemoveValuesByPatternAsync("users_*");

		await dbContext.SaveChangesAsync(cancellationToken);
	}
}