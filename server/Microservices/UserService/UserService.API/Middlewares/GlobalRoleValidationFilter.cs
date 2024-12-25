using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using UserService.Application.Interfaces.Auth;
using UserService.Domain.Enums;

public class GlobalRoleValidationFilter : IActionFilter
{
	public void OnActionExecuting(ActionExecutingContext context)
	{
		foreach (var argument in context.ActionArguments.Values)
		{
			if (argument is IHasRole roleRequest)
			{
				var role = roleRequest.Role;

				if (!Enum.TryParse<Role>(role, out var parsedRole))
				{
					context.Result = new BadRequestObjectResult("Such role does not exist");
					return;
				}

				if (parsedRole != Role.User)
				{
					context.Result = new BadRequestObjectResult("Role does not equal the necessary one");
					return;
				}
			}
		}
	}

	public void OnActionExecuted(ActionExecutedContext context)
	{
	}
}
