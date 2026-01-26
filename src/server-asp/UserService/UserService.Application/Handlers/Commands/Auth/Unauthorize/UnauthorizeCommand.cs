using MediatR;

namespace UserService.Application.Handlers.Commands.Auth.Unauthorize;

public record UnauthorizeCommand(Guid Id) : IRequest;
