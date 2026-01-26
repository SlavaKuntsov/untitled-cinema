using MediatR;

namespace UserService.Application.Handlers.Commands.Users.DeleteUser;

public partial class DeleteUserCommand(Guid id) : IRequest
{
    public Guid Id { get; private set; } = id;
}