using MediatR;

using UserService.Domain.Enums;

namespace UserService.Application.Handlers.Commands.Tokens.GenerateAccessToken;

public partial class GenerateAccessTokenCommand(
	Guid id,
	Role role) : IRequest<string>
{
	public Guid Id { get; private set; } = id;
	public Role Role { get; private set; } = role;
}