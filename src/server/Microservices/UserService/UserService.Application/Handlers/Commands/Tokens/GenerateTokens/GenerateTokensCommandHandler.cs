using Mapster;
using MapsterMapper;
using MediatR;
using UserService.Application.Data;
using UserService.Application.DTOs;
using UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Tokens.GenerateTokens;

public class GenerateTokensCommandHandler(
	ITokensRepository tokensRepository,
	IJwt jwt,
	IDBContext dbContext,
	IMapper mapper) : IRequestHandler<GenerateTokensCommand, AuthDto>
{
	public async Task<AuthDto> Handle(GenerateTokensCommand request, CancellationToken cancellationToken)
	{
		var accessToken = jwt.GenerateAccessToken(request.Id, request.Role);
		var newRefreshToken = jwt.GenerateRefreshToken();

		var newRefreshTokenModel = new RefreshTokenModel(
			request.Id,
			newRefreshToken,
			jwt.GetRefreshTokenExpirationDays());

		var existRefreshToken = await tokensRepository.GetAsync(
			request.Id,
			cancellationToken);

		if (existRefreshToken is not null)
		{
			newRefreshTokenModel.Id = existRefreshToken.Id;
			newRefreshTokenModel.Adapt(existRefreshToken);

			tokensRepository.Update(existRefreshToken);
		}
		else
		{
			await tokensRepository.CreateAsync(
				mapper.Map<RefreshTokenEntity>(newRefreshTokenModel),
				cancellationToken);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return new AuthDto(accessToken, newRefreshToken);
	}
}