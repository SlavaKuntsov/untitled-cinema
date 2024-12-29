using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Tokens;

public class GenerateAndUpdateTokensCommand(Guid id, Role role) : IRequest<AuthDto>
{
	public Guid Id { get; private set; } = id;
	public Role Role { get; private set; } = role;

	public class GenerateAndUpdateTokensCommandHandler(ITokensRepository tokensRepository, IJwt jwt) : IRequestHandler<GenerateAndUpdateTokensCommand, AuthDto>
	{
		private readonly ITokensRepository _tokensRepository = tokensRepository;
		private readonly IJwt _jwt = jwt;

		public async Task<AuthDto> Handle(GenerateAndUpdateTokensCommand request, CancellationToken cancellationToken)
		{
			var accessToken = _jwt.GenerateAccessToken(request.Id, request.Role);
			var newRefreshToken = _jwt.GenerateRefreshToken();

			var refreshTokenModel = new RefreshTokenModel(
				request.Id,
				request.Role,
				newRefreshToken,
				_jwt.GetRefreshTokenExpirationDays());

			await _tokensRepository.SetorUpdateRefreshTokenAsync(
				request.Id,
				request.Role,
				refreshTokenModel,
				cancellationToken);

			return new AuthDto(accessToken, newRefreshToken);
		}
	}
}