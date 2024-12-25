using MapsterMapper;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models.Users;

namespace UserService.Application.Handlers.Commands.Tokens;


public class RefreshTokenCommand(string refreshToken) : IRequest<UserDto>
{
	public string RefreshToken { get; private set; } = refreshToken;

	public class RefreshTokenCommandHandler(IUsersRepository usersRepository, IMapper mapper, IJwt jwt) : IRequestHandler<RefreshTokenCommand, UserDto>
	{
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly IMapper _mapper = mapper;
		private readonly IJwt _jwt = jwt;

		public async Task<UserDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
		{
			var userId = await _jwt.ValidateRefreshToken(request.RefreshToken, cancellationToken);

			if (userId == Guid.Empty)
				throw new InvalidTokenException("Invalid refresh token");

			UserModel? user = await _usersRepository.Get(userId, cancellationToken);

			if (user == null)
			{
				user = await _usersRepository.Get(userId, cancellationToken);

				if (user == null)
					throw new NotFoundException("User not found");
			}

			return _mapper.Map<UserDto>(user);
		}
	}
}
