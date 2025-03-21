using UserService.Domain.Enums;

namespace UserService.Application.DTOs;

public record UserRoleDto(
	Guid Id,
	Role Role);