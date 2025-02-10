using MediatR;

using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetAllUsers;

public partial class GetAllUsersQuery() : IRequest<IList<UserModel>>
{

}