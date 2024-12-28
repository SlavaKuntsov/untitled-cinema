using MapsterMapper;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models.Users;

namespace UserService.Application.Handlers.Commands.Tokens;


public class RefreshTokenCommand(string refreshToken) : IRequest<UserRoleDto>
{
	public string RefreshToken { get; private set; } = refreshToken;

	public class RefreshTokenCommandHandler(IUsersRepository usersRepository, IMapper mapper, IJwt jwt) : IRequestHandler<RefreshTokenCommand, UserRoleDto>
	{
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly IMapper _mapper = mapper;
		private readonly IJwt _jwt = jwt;

		public async Task<UserRoleDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
		{
			var userId = await _jwt.ValidateRefreshToken(request.RefreshToken, cancellationToken);

			if (userId == Guid.Empty)
				throw new InvalidTokenException("Invalid refresh token");

			Role role = await _usersRepository.GetRoleById(userId, cancellationToken)
				?? throw new NotFoundException("User not found");

			return new UserRoleDto(userId, role);
		}
	}
}