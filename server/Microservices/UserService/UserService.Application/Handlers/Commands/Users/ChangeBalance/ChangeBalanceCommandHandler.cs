using MapsterMapper;

using MediatR;

using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.ChangeBalance;

public class ChangeBalanceCommandHandler(
	IUsersRepository usersRepository,
	//IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<ChangeBalanceCommand, UserModel>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	//private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;

	public async Task<UserModel> Handle(ChangeBalanceCommand request, CancellationToken cancellationToken)
	{
		var existUser = await _usersRepository.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		if (request.Amount < existUser!.Balance)
			throw new InvalidOperationException($"User with id '{request.Id}' has a balance less than the booking cost.");

		if (request.IsIncrease)
			existUser.Balance += request.Amount;
		else
			existUser.Balance -= request.Amount;

		_usersRepository.Update(existUser);

		//await _redisCacheService.RemoveValuesByPatternAsync("users_*");

		return _mapper.Map<UserModel>(existUser);
	}
}