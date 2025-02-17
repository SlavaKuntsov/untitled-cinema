using MediatR;

using UserService.Application.DTOs;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserWithStringDateOfBirthDto?>;