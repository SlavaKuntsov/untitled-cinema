using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Queries.Tokens.GetByRefreshToken;

public partial class GetByRefreshTokenCommand(string refreshToken) : IRequest<UserRoleDto>
{
	public string RefreshToken { get; private set; } = refreshToken;
}