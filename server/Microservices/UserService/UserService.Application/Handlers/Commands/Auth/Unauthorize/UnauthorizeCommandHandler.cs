using MediatR;

using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Auth.Unauthorize;

public class UnauthorizeCommandHandler(ITokensRepository tokensRepository) : IRequestHandler<UnauthorizeCommand>
{
	private readonly ITokensRepository _tokensRepository = tokensRepository;

	public async Task Handle(UnauthorizeCommand request, CancellationToken cancellationToken)
	{
		var existRefreshToken = await _tokensRepository.GetAsync(
			request.Id,
			cancellationToken);

		existRefreshToken!.IsRevoked = true;

		_tokensRepository.Update(existRefreshToken);

		return;
	}
}