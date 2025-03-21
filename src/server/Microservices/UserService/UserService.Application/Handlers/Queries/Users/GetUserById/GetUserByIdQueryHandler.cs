using MapsterMapper;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces.Caching;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.GetUserById;

public class GetUserByIdQueryHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserWithStringDateOfBirthDto?>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;

	public async Task<UserWithStringDateOfBirthDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var cacheKey = $"users_{request.Id}";

		var cachedProfile = await _redisCacheService
			.GetValueAsync<UserWithStringDateOfBirthDto>(cacheKey);

		if (cachedProfile != null)
		{
			return new UserWithStringDateOfBirthDto(
				cachedProfile.Id,
				cachedProfile.Email,
				cachedProfile.Role,
				cachedProfile.FirstName,
				cachedProfile.LastName,
				cachedProfile.DateOfBirth,
				cachedProfile.Balance);
		}

		var entity = await _usersRepository.GetWithStringDateOfBirthAsync(
			request.Id,
			cancellationToken)
			?? throw new NotFoundException("User not found");

		var dto = _mapper.Map<UserWithStringDateOfBirthDto>(entity);

		await _redisCacheService.SetValueAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

		return dto;
	}
}