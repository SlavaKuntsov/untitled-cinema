using Domain.Exceptions;

using Extensions.Strings;

using Mapster;
using MapsterMapper;
using MediatR;
using Redis.Services;
using UserService.Application.Data;
using UserService.Application.DTOs;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

public class UpdateUserCommandHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IDBContext dbContext,
	IMapper mapper) : IRequestHandler<UpdateUserCommand, UserDto>
{
	public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		var userId = request.Id 
			?? throw new ArgumentNullException(nameof(request.Id), "User ID is required.");

		var existUser = await usersRepository.GetAsync(userId, cancellationToken)
			?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		request.Adapt(existUser);

		usersRepository.Update(existUser);

		await redisCacheService.RemoveValuesByPatternAsync("users_*");
		
		await dbContext.SaveChangesAsync(cancellationToken);

		return mapper.Map<UserDto>(existUser);
	}
}