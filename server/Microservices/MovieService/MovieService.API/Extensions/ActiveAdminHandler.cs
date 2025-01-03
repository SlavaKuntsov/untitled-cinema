using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authorization;

using MovieService.Domain.Interfaces.Grpc;

namespace MovieService.API.Extensions;

public class ActiveAdminHandler : AuthorizationHandler<ActiveAdminRequirement>
{
	private readonly IMediator _mediator;
	private readonly IAuthGrpcService _authGrpcService;

	public ActiveAdminHandler(IMediator mediator, IAuthGrpcService authGrpcService)
	{
		_mediator = mediator;
		_authGrpcService = authGrpcService;
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
			var admin = await _authGrpcService.CheckExistAsync(userId, CancellationToken.None);

			if (!admin)
			{
				context.Fail(); // Администратор не найден
				return;
			}
		}

		context.Succeed(requirement);
	}
}