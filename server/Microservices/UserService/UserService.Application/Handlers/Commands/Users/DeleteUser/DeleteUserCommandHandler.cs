using MediatR;

using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(
	IUsersRepository usersRepository,
	ITokensRepository tokensRepository) : IRequestHandler<DeleteUserCommand>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly ITokensRepository _tokensRepository = tokensRepository;

	public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _usersRepository.GetWithTokenAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		if(user.RefreshToken is null)
			throw new NotFoundException($"Refresh Token for user with id {request.Id} not found");

		if (user.Role is Role.User)
		{
			_usersRepository.Delete(user, user.RefreshToken);
		}
		else if (user.Role is Role.Admin)
		{
			var admins = await _usersRepository.GetByRole(Role.Admin, cancellationToken);

			if (admins.Count == 1)
				throw new UnprocessableContentException("Cannot delete the last Admin");

			_usersRepository.Delete(user, user.RefreshToken);
		}

		return;
	}
}