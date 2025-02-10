using MediatR;

using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetUser;

public partial class GetUserByIdQuery(Guid id) : IRequest<UserModel?>
{
    public Guid Id { get; init; } = id;
}