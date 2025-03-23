using Domain.Enums;
using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;

public partial class GenerateTokensCommand(Guid id, Role role) : IRequest<AuthDto>
{
    public Guid Id { get; private set; } = id;
    public Role Role { get; private set; } = role;
}