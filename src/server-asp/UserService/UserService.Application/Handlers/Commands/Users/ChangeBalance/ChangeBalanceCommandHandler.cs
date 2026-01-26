using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Redis.Services;
using UserService.Application.Data;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.ChangeBalance;

public class ChangeBalanceCommandHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IDBContext dbContext,
	IMapper mapper) : IRequestHandler<ChangeBalanceCommand, UserModel>
{
	public async Task<UserModel> Handle(ChangeBalanceCommand request, CancellationToken cancellationToken)
	{
		var existUser = await usersRepository.GetAsync(request.Id, cancellationToken)
						?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		if (request.Amount < existUser!.Balance)
			throw new InvalidOperationException(
				$"User with id '{request.Id}' has a balance less than the booking cost.");

		if (request.IsIncrease)
			existUser.Balance += request.Amount;
		else
			existUser.Balance -= request.Amount;

		usersRepository.Update(existUser);

		await redisCacheService.RemoveValuesByPatternAsync("users_*");
		
		await dbContext.SaveChangesAsync(cancellationToken);

		return mapper.Map<UserModel>(existUser);
	}
}