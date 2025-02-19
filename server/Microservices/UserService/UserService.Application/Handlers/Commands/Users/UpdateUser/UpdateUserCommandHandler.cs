using Mapster;

using MapsterMapper;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Extensions;
using UserService.Application.Interfaces.Caching;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

public class UpdateUserCommandHandler(
	IUsersRepository usersRepository,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<UpdateUserCommand, UserDto>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;

	public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		if (!request.DateOfBirth.DateFormatTryParse(out DateTime parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		var userId = request.Id ??
			throw new ArgumentNullException(nameof(request.Id), "User ID is required.");

		var existUser = await _usersRepository.GetAsync(userId, cancellationToken)
			?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		request.Adapt(existUser);

		_usersRepository.Update(existUser);

		await _redisCacheService.RemoveValuesByPatternAsync("users_*");

		return _mapper.Map<UserDto>(existUser);
	}
}