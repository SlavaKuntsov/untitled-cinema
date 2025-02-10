using MediatR;

using UserService.Application.Interfaces.Auth;

namespace UserService.Application.Handlers.Commands.Tokens.GenerateAccessToken;

public class GenerateAccessTokenCommandHandler(IJwt jwt) : IRequestHandler<GenerateAccessTokenCommand, string>
{
	private readonly IJwt _jwt = jwt;

	public async Task<string> Handle(GenerateAccessTokenCommand request, CancellationToken cancellationToken)
	{
		return _jwt.GenerateAccessToken(request.Id, request.Role);
	}
}