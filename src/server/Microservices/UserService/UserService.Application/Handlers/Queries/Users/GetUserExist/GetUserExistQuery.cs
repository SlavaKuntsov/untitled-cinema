using MediatR;

namespace UserService.Application.Handlers.Queries.Users.GetUserExist;

public partial class GetUserExistQuery(Guid id) : IRequest<bool>
{
	public Guid Id { get; init; } = id;
}