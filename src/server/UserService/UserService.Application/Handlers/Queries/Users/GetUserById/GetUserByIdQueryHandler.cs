using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Redis.Services;
using UserService.Application.DTOs;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.GetUserById;

public class GetUserByIdQueryHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
	public async Task<UserDto?> Handle(
		GetUserByIdQuery request,
		CancellationToken cancellationToken)
	{
		var cacheKey = $"users_{request.Id}";

		var cachedProfile = await redisCacheService
			.GetValueAsync<UserDto>(cacheKey);

		if (cachedProfile != null)
			return new UserDto(
				cachedProfile.Id,
				cachedProfile.Email,
				cachedProfile.Role,
				cachedProfile.FirstName,
				cachedProfile.LastName,
				cachedProfile.DateOfBirth,
				cachedProfile.Balance);

		var entity = await usersRepository.GetAsync(
						request.Id,
						cancellationToken)
					?? throw new NotFoundException("User not found");

		var dto = mapper.Map<UserDto>(entity);

		await redisCacheService.SetValueAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

		return dto;
	}
}