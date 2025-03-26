using Domain.Constants;

using Microsoft.AspNetCore.Http;

namespace Utilities.Service;

public class CookieService(IHttpContextAccessor httpContextAccessor) : ICookieService
{
	public string GetRefreshToken()
	{
		var httpContext = httpContextAccessor.HttpContext;

		if (httpContext == null)
			throw new InvalidOperationException("No active HTTP context available.");

		if (httpContext.Request.Cookies.TryGetValue(JwtConstants.REFRESH_COOKIE_NAME, out var refreshToken))
			return refreshToken;

		throw new InvalidOperationException("Refresh token not found in cookies.");
	}

	public void DeleteRefreshToken()
	{
		var httpContext = httpContextAccessor.HttpContext 
			?? throw new InvalidOperationException("No active HTTP context available.");

		httpContext.Response.Cookies.Delete(JwtConstants.REFRESH_COOKIE_NAME);
	}
}