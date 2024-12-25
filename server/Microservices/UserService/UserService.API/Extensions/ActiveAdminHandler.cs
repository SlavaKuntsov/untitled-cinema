using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authorization;

using UserService.Application.Handlers.Queries.Users;

namespace UserService.API.Extensions;

public class ActiveAdminHandler : AuthorizationHandler<ActiveAdminRequirement>
{
	private readonly IMediator _mediator;

	public ActiveAdminHandler(IMediator mediator)
	{
		_mediator = mediator;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveAdminRequirement requirement)
	{
		var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

		if (userIdClaim == null)
		{
			context.Fail();
			return;
		}

		Guid userId = Guid.Parse(userIdClaim.Value);

		if (context.User.IsInRole("Admin"))
		{
			var admin = await _mediator.Send(new GetUserQuery(userId));

			if (admin == null)
			{
				context.Fail(); // Администратор не найден
				return;
			}

		}

		context.Succeed(requirement);
	}
}