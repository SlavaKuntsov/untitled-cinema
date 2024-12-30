using MapsterMapper;

using MediatR;

using UserService.Application.Interfaces.Auth;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Commands.Tokens.GenerateAccessToken;

public class GenerateAccessTokenCommandHandler(ITokensRepository tokensRepository, IJwt jwt, IMapper mapper) : IRequestHandler<GenerateAccessTokenCommand, string>
{
	private readonly ITokensRepository _tokensRepository = tokensRepository;
	private readonly IJwt _jwt = jwt;
	private readonly IMapper _mapper = mapper;

	public async Task<string> Handle(GenerateAccessTokenCommand request, CancellationToken cancellationToken)
	{
		return _jwt.GenerateAccessToken(request.Id, request.Role);
	}
}