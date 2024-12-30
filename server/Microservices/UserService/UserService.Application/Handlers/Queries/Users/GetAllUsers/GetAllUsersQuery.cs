using MediatR;

using UserService.Domain;

namespace UserService.Application.Handlers.Queries.Users.GetAllUsers;

public partial class GetAllUsersQuery() : IRequest<IList<UserModel>>
{

}