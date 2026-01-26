using MediatR;

using UserService.Application.DTOs;
using UserService.Domain;

namespace UserService.Application.Handlers.Queries.Users.Login;

public partial class LoginQuery(string email, string password) : IRequest<UserRoleDto>
{
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
}