using MediatR;

using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(IUsersRepository usersRepository) : IRequestHandler<DeleteUserCommand>
{
	private readonly IUsersRepository _usersRepository = usersRepository;

	public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _usersRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		await _usersRepository.DeleteAsync(user, cancellationToken);
		return;
	}
}
