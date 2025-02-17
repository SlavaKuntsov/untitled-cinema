using MediatR;

using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Auth.Unauthorize;

public class UnauthorizeCommandHandler(
	ITokensRepository tokensRepository
	//IRedisCacheService redisCacheService,
	) : IRequestHandler<UnauthorizeCommand>
{
	private readonly ITokensRepository _tokensRepository = tokensRepository;
	//private readonly IRedisCacheService _redisCacheService = redisCacheService;

	public async Task Handle(UnauthorizeCommand request, CancellationToken cancellationToken)
	{
		var existRefreshToken = await _tokensRepository.GetAsync(
			request.Id,
			cancellationToken);

		existRefreshToken!.IsRevoked = true;

		_tokensRepository.Update(existRefreshToken);

		//await _redisCacheService.RemoveValuesByPatternAsync("users_*");

		return;
	}
}