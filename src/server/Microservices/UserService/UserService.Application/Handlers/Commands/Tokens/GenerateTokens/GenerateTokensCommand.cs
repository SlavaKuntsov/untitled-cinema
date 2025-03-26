using Domain.Enums;
using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Commands.Tokens.GenerateAndUpdateTokens;

public record GenerateTokensCommand(Guid Id, Role Role) : IRequest<AuthDto>;
