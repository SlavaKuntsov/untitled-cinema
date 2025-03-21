using Microsoft.AspNetCore.Authorization;

namespace MovieService.API.Extensions;

public class ActiveAdminRequirement : IAuthorizationRequirement
{
	public ActiveAdminRequirement() 
	{ 
	}
}