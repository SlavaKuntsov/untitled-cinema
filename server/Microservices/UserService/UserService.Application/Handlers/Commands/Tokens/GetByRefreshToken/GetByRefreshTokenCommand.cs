using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Commands.Tokens.RefreshToken;

public partial class GetByRefreshTokenCommand(string refreshToken) : IRequest<UserRoleDto>
{
    public string RefreshToken { get; private set; } = refreshToken;
}