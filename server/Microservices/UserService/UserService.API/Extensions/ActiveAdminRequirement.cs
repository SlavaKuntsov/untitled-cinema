using Microsoft.AspNetCore.Authorization;

namespace UserService.API.Extensions;

public class ActiveAdminRequirement : IAuthorizationRequirement
{
	public ActiveAdminRequirement() 
	{ 
	
	}
}