using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Redis.Service;
using UserService.Application.DTOs;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.GetUserById;

public class GetUserByIdQueryHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserWithStringDateOfBirthDto?>
{
	public async Task<UserWithStringDateOfBirthDto?> Handle(
		GetUserByIdQuery request,
		CancellationToken cancellationToken)
	{
		var cacheKey = $"users_{request.Id}";

		var cachedProfile = await redisCacheService
			.GetValueAsync<UserWithStringDateOfBirthDto>(cacheKey);

		if (cachedProfile != null)
			return new UserWithStringDateOfBirthDto(
				cachedProfile.Id,
				cachedProfile.Email,
				cachedProfile.Role,
				cachedProfile.FirstName,
				cachedProfile.LastName,
				cachedProfile.DateOfBirth,
				cachedProfile.Balance);

		var entity = await usersRepository.GetWithStringDateOfBirthAsync(
						request.Id,
						cancellationToken)
					?? throw new NotFoundException("User not found");

		var dto = mapper.Map<UserWithStringDateOfBirthDto>(entity);

		await redisCacheService.SetValueAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

		return dto;
	}
}