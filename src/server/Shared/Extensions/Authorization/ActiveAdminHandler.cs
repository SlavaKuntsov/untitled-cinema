using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace UserService.API.Extensions;

public class ActiveAdminHandler : AuthorizationHandler<ActiveAdminRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		ActiveAdminRequirement requirement)
	{
		// Проверяем, есть ли claim с идентификатором пользователя
		var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

		if (userIdClaim == null)
		{
			context.Fail(); // Нет claim с идентификатором пользователя

			return Task.CompletedTask;
		}

		// Проверяем, есть ли у пользователя роль "Admin"
		if (context.User.IsInRole("Admin"))
			context.Succeed(requirement); // Пользователь является администратором
		else
			context.Fail(); // Пользователь не является администратором

		return Task.CompletedTask;
	}
}

public class ActiveAdminRequirement : IAuthorizationRequirement;