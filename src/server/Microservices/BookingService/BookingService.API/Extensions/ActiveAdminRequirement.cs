using Microsoft.AspNetCore.Authorization;

namespace BookingService.API.Extensions;

public class ActiveAdminRequirement : IAuthorizationRequirement
{
	public ActiveAdminRequirement() 
	{ 
	}
}