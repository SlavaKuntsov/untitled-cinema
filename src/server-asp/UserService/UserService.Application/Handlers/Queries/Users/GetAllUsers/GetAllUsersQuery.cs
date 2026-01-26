using MediatR;

using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetAllUsers;

public record GetAllUsersQuery() : IRequest<IList<UserModel>>;