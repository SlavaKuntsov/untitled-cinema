using MapsterMapper;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.Login;

public class LoginQueryHandler(
	IUsersRepository usersRepository,
	IPasswordHash passwordHash,
	IMapper mapper) : IRequestHandler<LoginQuery, UserRoleDto>
{
	private readonly IPasswordHash _passwordHash = passwordHash;
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<UserRoleDto> Handle(LoginQuery request, CancellationToken cancellationToken)
	{
		var existUser = await _usersRepository.GetAsync(request.Email, cancellationToken)
				?? throw new NotFoundException("User not found");

		var isCorrectPassword = _passwordHash.Verify(request.Password, existUser.Password);

		if (!isCorrectPassword)
			throw new UnauthorizedAccessException("Incorrect password");

		return _mapper.Map<UserRoleDto>(existUser);
	}
}