using MediatR;

using UserService.Application.DTOs;

namespace UserService.Application.Handlers.Queries.Users.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserWithStringDateOfBirthDto?>;