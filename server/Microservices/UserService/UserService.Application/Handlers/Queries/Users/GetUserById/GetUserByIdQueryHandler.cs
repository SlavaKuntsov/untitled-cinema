using System.Text.Json;

using MapsterMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Caching;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetUserById;

public class GetUserByIdQueryHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IMapper mapper,
	ILogger<GetUserByIdQueryHandler> logger) : IRequestHandler<GetUserByIdQuery, UserWithStringDateOfBirthDto?>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;
	private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;

	public async Task<UserWithStringDateOfBirthDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var cacheKey = $"users_{request.Id}";

		var cachedProfile = await _redisCacheService
			.GetValueAsync<UserWithStringDateOfBirthDto>(cacheKey);

		if (cachedProfile != null)
		{
			_logger.LogError("FROM CACHE");
			
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

		_logger.LogError("FROM DB");

		return dto;
	}
}