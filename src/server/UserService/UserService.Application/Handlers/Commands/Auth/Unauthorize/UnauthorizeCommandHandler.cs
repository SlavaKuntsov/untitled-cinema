using MediatR;
using Redis.Service;
using UserService.Application.Data;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Auth.Unauthorize;

public class UnauthorizeCommandHandler(
	ITokensRepository tokensRepository,
	IRedisCacheService redisCacheService,
	IDBContext dbContext) : IRequestHandler<UnauthorizeCommand>
{
	public async Task Handle(UnauthorizeCommand request, CancellationToken cancellationToken)
	{
		var existRefreshToken = await tokensRepository.GetAsync(
			request.Id,
			cancellationToken);

		existRefreshToken!.IsRevoked = true;

		tokensRepository.Update(existRefreshToken);
		
		await dbContext.SaveChangesAsync(cancellationToken);

		await redisCacheService.RemoveValuesByPatternAsync("users_*");
	}
}